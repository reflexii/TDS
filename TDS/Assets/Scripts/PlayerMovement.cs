using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    private GameManager gameManager;
    public float movementSpeed;
    public bool drawPlayerGizmos = true;
    private float rotateSpeed = 500f;

    //Raycasting and movement
    private bool disableTop = false;
    private bool disableBottom = false;
    private bool disableLeft = false;
    private bool disableRight = false;

    //Gun
    public GameObject gun;

    //Grenade
    public float howFastGrenadeSpeedIncreases = 3f;
    private float increasedGrenadeSpeed = 0f;
    public GameObject grenadePrefab;
    public int startGrenadeAmount;
    [HideInInspector]
    public int currentGrenadeAmount;
    [HideInInspector]
    public Text grenadeText;

    //Knife
    private Knife knife;
    public float knifeDamage = 5f;
    private float knifeTimer = 0f;

    //Health
    public float playerHealth = 100f;
    public float playerCurrentHealth;
    public bool dead = false;
    public GameObject dieBloodSmallPrefab;
    public GameObject dieBloodBigPrefab;

    //Gunswitching
    public bool gunInRange = false;
    private GameObject floorGun;
    public bool hasGun;
    public bool gunDeactivated = false;

    //hud sprites
    public Sprite knifeSprite;
    public Image currentGunImage;
    public Text magazineText;

    //use + buttons
    public bool togglable = false;
    private GameObject togglableButton;

    //animations
    private Animator animator;
    private bool walking;
    //1=pistol 2=shotgun 3=smg 4=rifle
    private int weaponUsed;
    private bool swingKnife = false;

    //grenadebar
    private Image grenadeBar;
    private Image grenadeBar_bg;
    private bool didOnce = false;

    //dialogue
    private float runningDialogueTimer = 0f;
    public float dialogueTime;
    private bool dialogueTogglable = false;
    private GameObject togglableDialogueObject;

    private Camera mainCamera;
    private GameObject bulletSpawnPoint;
    private GameObject reticle;

    //UseHelper
    private Image useHelper;
    private Image rightClickIndicator;
    private bool showE = false;

    //health
    private Image healthImage;
    private Text healthText;
    private bool lowHealth = false;

    //corpses
    public GameObject playerCorpse1;
    public GameObject playerCorpse2;
    public GameObject playerCorpse3;

    //vip use
    private bool vipTogglable = false;
    private GameObject vipObject;

    //objectives
    private ObjectiveManager objectiveManager;
    private bool fail = false;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.GetComponent<GameManager>().player = gameObject;
        magazineText = GameObject.Find("MagazineText").GetComponent<Text>();
        grenadeText = GameObject.Find("GrenadeText").GetComponent<Text>();
        currentGunImage = GameObject.Find("GunImage").GetComponent<Image>();
        knife = transform.Find("KnifeSwingCollider").gameObject.GetComponent<Knife>();
        grenadeBar = GameObject.Find("grenadeBar").GetComponent<Image>();
        grenadeBar_bg = GameObject.Find("grenadeBar_bg").GetComponent<Image>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        useHelper = GameObject.Find("UseHelper").GetComponent<Image>();
        rightClickIndicator = GameObject.Find("RightClickIndicator").GetComponent<Image>();
        bulletSpawnPoint = transform.Find("BulletSpawnPoint").gameObject;
        reticle = GameObject.Find("Reticle");
        healthImage = GameObject.Find("HealthImage").GetComponent<Image>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        objectiveManager = GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>();
        animator = GetComponent<Animator>();

        Color temp = GetComponent<SpriteRenderer>().color;
        temp.a = 255f;
        GetComponent<SpriteRenderer>().color = temp;

        grenadeBar.fillAmount = 0f;
        grenadeBar_bg.fillAmount = 0f;
        grenadeBar.enabled = false;
        grenadeBar_bg.enabled = false;
        useHelper.enabled = false;

        if (hasGun)
        {
            gun = transform.Find("Gun").gameObject;
        } else
        {
            Destroy(transform.Find("Gun").gameObject, 0f);
        }

        currentGrenadeAmount = startGrenadeAmount;
        grenadeText.text = "" + currentGrenadeAmount;
        playerCurrentHealth = playerHealth;

    }

    void Update () {

        if (dead || objectiveManager.missionFailed) {
            if (objectiveManager.missionFailed) {
                useHelper.enabled = false;
                rightClickIndicator.enabled = false;
                fail = true;
                animator.SetBool("MissionFailed", fail);
                reticle.SetActive(false);
                if (hasGun) {
                    gun.GetComponent<Gun>().fail = true;
                }
            }
        } else {
            Movement();
            MouseLook();
            WallCheck();
            Shooting();
            Grenade();
            GunSwitching();
            KnifeSprite();
            Use();
            Animations();
            UseHelper();
            RightClickIndicator();
            reticle.SetActive(true);
        }

        HealthUpdate();

    }

    void HealthUpdate() {
        healthText.text = "" + playerCurrentHealth;

        if (playerCurrentHealth < 0f) {
            playerCurrentHealth = 0f;
        }

        if (playerCurrentHealth <= 50f) {
            lowHealth = true;
            healthText.gameObject.GetComponent<Animator>().SetBool("LowHealth", lowHealth);
        }
    }

    void Animations() {
        animator.SetBool("Walking", walking);
        animator.SetBool("GunDeactivated", gunDeactivated);
        animator.SetBool("HasGun", hasGun);
        animator.SetInteger("WeaponUsed", weaponUsed);
        animator.SetBool("SwingKnife", swingKnife);
        animator.SetBool("MissionFailed", fail);
        healthText.gameObject.GetComponent<Animator>().SetBool("LowHealth", lowHealth);

        //1=pistol 2=shotgun 3=smg 4=rifle
        if (hasGun) {
            switch (transform.Find("Gun").GetComponent<Gun>().weaponInUse) {
                case Gun.Weapons.Pistol:
                    weaponUsed = 1;
                    break;
                case Gun.Weapons.Shotgun:
                    weaponUsed = 2;
                    break;
                case Gun.Weapons.SMG:
                    weaponUsed = 3;
                    break;
                case Gun.Weapons.Rifle:
                    weaponUsed = 4;
                    break;
            }
        }
    }

    void UseHelper() {
        Vector3 pos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(transform.position) + new Vector3(0f, 55f, 0f);
        useHelper.transform.position = pos;

        if (showE || vipTogglable) {
            useHelper.enabled = true;
        } else {
            useHelper.enabled = false;
        }
    }

    void RightClickIndicator() {
        Vector3 pos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(transform.position) + new Vector3(0f, 55f, 0f);
        rightClickIndicator.transform.position = pos;

        if (gunInRange) {
            rightClickIndicator.enabled = true;
        } else {
            rightClickIndicator.enabled = false;
        }
    }

    void KnifeSprite()
    {
        if (!hasGun || gunDeactivated)
        {
            currentGunImage.sprite = knifeSprite;
            magazineText.text = "";
        }
    }

    void Movement()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !disableTop && !disableRight) {
            transform.position += new Vector3(1f, 1f).normalized * movementSpeed * Time.deltaTime;
            walking = true;
        } else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !disableTop && !disableLeft) {
            transform.position += new Vector3(-1f, 1f).normalized * movementSpeed * Time.deltaTime;
            walking = true;
        } else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !disableBottom && !disableRight) {
            transform.position += new Vector3(1f, -1f).normalized * movementSpeed * Time.deltaTime;
            walking = true;
        } else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && !disableBottom && !disableLeft) {
            transform.position += new Vector3(-1f, -1f).normalized * movementSpeed * Time.deltaTime;
            walking = true;
        } else {
            if (Input.GetKey(KeyCode.W) && !disableTop) {
                transform.position += new Vector3(0f, 1f) * movementSpeed * Time.deltaTime;
                walking = true;
            }
             else if (Input.GetKey(KeyCode.S) && !disableBottom) {
                transform.position += new Vector3(0f, -1f) * movementSpeed * Time.deltaTime;
                walking = true;
            }
            else if (Input.GetKey(KeyCode.A) && !disableLeft) {
                transform.position += new Vector3(-1f, 0f) * movementSpeed * Time.deltaTime;
                walking = true;
            }
            else if (Input.GetKey(KeyCode.D) && !disableRight) {
                transform.position += new Vector3(1f, 0f) * movementSpeed * Time.deltaTime;
                walking = true;
            } else {
                walking = false;
            }
        }
    }

    void Grenade()
    {

        if (Input.GetKey(KeyCode.G) && currentGrenadeAmount > 0)
        {

            Vector3 pos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(transform.position) + new Vector3(0f, 45f, 0f);
            grenadeBar.transform.position = pos;
            grenadeBar_bg.transform.position = pos;

            increasedGrenadeSpeed += Time.deltaTime * howFastGrenadeSpeedIncreases;
            if (increasedGrenadeSpeed >= 6f)
            {
                increasedGrenadeSpeed = 6f;
            }

            if (!didOnce) {
                
                grenadeBar.enabled = true;
                grenadeBar_bg.enabled = true;
                didOnce = true;
            }

            grenadeBar.fillAmount = (increasedGrenadeSpeed/6f);
        }
        if (Input.GetKeyUp(KeyCode.G) && currentGrenadeAmount > 0)
        {

            // convert mouse position into world coordinates
            Vector2 mouseScreenPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // get direction you want to point at
            Vector2 direction = (mouseScreenPosition - (Vector2)transform.position).normalized;

            GameObject gre = Instantiate<GameObject>(grenadePrefab, gameObject.transform.position, Quaternion.identity);
            Grenade g = gre.GetComponent<Grenade>();
            g.grenadeMoveDirection = direction;
            g.grenadeSpeed += increasedGrenadeSpeed;

            currentGrenadeAmount--;
            grenadeText.text = "" + currentGrenadeAmount;
            increasedGrenadeSpeed = 0f;

            grenadeBar.enabled = false;
            grenadeBar_bg.enabled = false;
            didOnce = false;
        }
    }

    void GunSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (gunInRange && hasGun)
            {
                DropGun();
                PickUpGun();
            } else if (!gunInRange && hasGun)
            {
                DropGun();
            } else if (gunInRange && !hasGun)
            {
                PickUpGun();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && hasGun) {
            gun.SetActive(gunDeactivated);
            gunDeactivated = !gunDeactivated;
        }
    }

    public void TakeDamage(float amount) {
        //blood
        Instantiate<GameObject>(dieBloodSmallPrefab, transform.position, Quaternion.identity);
        //sound

        playerCurrentHealth -= amount;

        if (playerCurrentHealth <= 0f && !dead) {
            Die();
        }
    }

    public void Die() {
        reticle.SetActive(false);
        useHelper.enabled = false;
        rightClickIndicator.enabled = false;
        dead = true;
        GenerateCorpse();
        playerCurrentHealth = 0f;
        healthText.text = "" + playerCurrentHealth;
        //Corpse
        //Blood
        Instantiate<GameObject>(dieBloodBigPrefab, transform.position, Quaternion.identity);
        Instantiate<GameObject>(dieBloodSmallPrefab, transform.position, Quaternion.identity);
        if (hasGun) {
            DropGun();
        }

        GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().MissionFailed("You have died.");

        //Sound

        //Death
        gameManager.playerIsDead = true;
        gameObject.SetActive(false);
    }

    public void GenerateCorpse() {
        int randomize = Random.Range(0, 3);
        GameObject chosenCorpse = null;

        if (randomize == 0) {
            chosenCorpse = playerCorpse1;
        } else if (randomize == 1) {
            chosenCorpse = playerCorpse2;
        } else {
            chosenCorpse = playerCorpse3;
        }

        Instantiate<GameObject>(chosenCorpse, transform.position, Quaternion.identity);

    }

    void DropGun()
    {
        gun.SetActive(true);
        gunDeactivated = false;
        gun.GetComponent<Collider2D>().enabled = true;
        // convert mouse position into world coordinates
        Vector2 mouseScreenPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at

        Vector3 dir = (mouseScreenPosition - (Vector2)transform.position).normalized;
        gun.GetComponent<Gun>().throwDirection = dir;
        gun.GetComponent<Gun>().throwWeapon = true;
        gun.transform.position += -dir * 0.3f;
        
        gun.transform.parent = null;
        gun.GetComponent<Gun>().playerOwned = false;
        gun.GetComponent<Gun>().gunOnTheFloor = true;
        gun.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gun = null;
        gunInRange = false;
        hasGun = false;
        transform.Find("BulletSpawnPoint/Muzzle").GetComponent<SpriteRenderer>().enabled = false;
    }

    void PickUpGun()
    {
        if (!hasGun)
        {
            gunInRange = false;
        }
        
        hasGun = true;
        gun = floorGun;
        gun.GetComponent<Gun>().throwWeapon = false;
        gun.transform.rotation = new Quaternion(0f, 0f, 0f , 0f);
        gun.transform.parent = gameObject.transform;
        gun.transform.position = gameObject.transform.position;
        gun.GetComponent<Gun>().playerOwned = true;
        gun.GetComponent<Gun>().gunOnTheFloor = false;
        gun.GetComponent<Gun>().bulletSpawnPoint = transform.Find("BulletSpawnPoint");
        gun.GetComponent<Gun>().FixBulletSpawnPoints();
        gun.GetComponent<Collider2D>().enabled = false;
        transform.Find("BulletSpawnPoint/Muzzle").GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Gun") && collision.gameObject.GetComponent<Gun>().gunOnTheFloor == true)
        {
            gunInRange = true;
            floorGun = collision.gameObject;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Button")) {
            togglable = true;
            togglableButton = collision.gameObject;
            showE = togglableButton.GetComponent<Button>().showE;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Dialogue")) {
            dialogueTogglable = true;
            togglableDialogueObject = collision.gameObject;
            showE = togglableDialogueObject.GetComponent<DialogueToggle>().showE;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Button"))
        {
            togglable = true;
            togglableButton = collision.gameObject;
            showE = togglableButton.GetComponent<Button>().showE;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Dialogue")) {
            dialogueTogglable = true;
            togglableDialogueObject = collision.gameObject;
            showE = togglableDialogueObject.GetComponent<DialogueToggle>().showE;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("VIP")) {
            vipTogglable = true;
            vipObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Gun") && collision.gameObject.GetComponent<Gun>().gunOnTheFloor == true)
        {
            gunInRange = false;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Button"))
        {
            togglable = false;
            showE = false;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Dialogue")) {
            dialogueTogglable = false;
            showE = false;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("VIP")) {
            vipTogglable = false;
        }
    }

    void MouseLook() {
        // convert mouse position into world coordinates
        Vector2 mouseScreenPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        Vector2 direction = (mouseScreenPosition - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);

        float dist = Vector3.Distance(bulletSpawnPoint.transform.position, mouseScreenPosition);

        reticle.transform.position = bulletSpawnPoint.transform.position + (Vector3)direction * dist;
    }

    void WallCheck() {

        int mask = 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("PlayerBlocker");

        if (Input.GetKey(KeyCode.W)) {
            if (Physics2D.Raycast(transform.position + new Vector3(0.3f, 0f, 0f), Vector3.up, 0.5f, mask) || Physics2D.Raycast(transform.position - new Vector3(0.3f, 0f, 0f), Vector3.up, 0.5f, mask)) {
                disableTop = true;
            } else {
                disableTop = false;
            }
        }
        if (Input.GetKey(KeyCode.S)) {
            if (Physics2D.Raycast(transform.position + new Vector3(0.3f, 0f, 0f), Vector3.down, 0.5f, mask) || Physics2D.Raycast(transform.position - new Vector3(0.3f, 0f, 0f), Vector3.down, 0.5f, mask)) {
                disableBottom = true;
            } else {
                disableBottom = false;
            }
        }
        if (Input.GetKey(KeyCode.A)) {
            if (Physics2D.Raycast(transform.position + new Vector3(0f, 0.3f, 0f), Vector3.left, 0.5f, mask) || Physics2D.Raycast(transform.position - new Vector3(0f, 0.3f, 0f), Vector3.left, 0.5f, mask)) {
                disableLeft = true;
            } else {
                disableLeft = false;
            }
        }    
        if (Input.GetKey(KeyCode.D)) {
            if (Physics2D.Raycast(transform.position + new Vector3(0f, 0.3f, 0f), Vector3.right, 0.5f, mask) || Physics2D.Raycast(transform.position - new Vector3(0f, 0.3f, 0f), Vector3.right, 0.5f, mask)) {
                disableRight = true;
            } else {
                disableRight = false;
            }
        }
        
    }

    void Use()
    {
        if (Input.GetKeyDown(KeyCode.E) && togglable && !togglableButton.GetComponent<Button>().walkTrigger)
        {
            togglableButton.GetComponent<Button>().Toggle();
        }

        if (Input.GetKeyDown(KeyCode.E) && dialogueTogglable) {
            if (togglableDialogueObject.GetComponent<DialogueToggle>().isDone) {
                togglableDialogueObject.gameObject.GetComponent<DialogueToggle>().TriggerDialogue();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.E) && vipTogglable) {
            vipObject.GetComponent<VIP>().following = !vipObject.GetComponent<VIP>().following;
        }
    }

    void Shooting() {
        if (swingKnife) {
            swingKnife = false;
        }
        knifeTimer += Time.deltaTime;

        if (gameManager.GetComponent<DialogManager>().DialogueActive()) {
            runningDialogueTimer += Time.deltaTime;
            if (Input.GetKeyUp(KeyCode.Mouse0) && runningDialogueTimer >= dialogueTime) {
                gameManager.GetComponent<DialogManager>().ToggleDialogueUIOff();
                runningDialogueTimer = 0f;
            }
        }
        else if (Input.GetKey(KeyCode.Mouse0) && hasGun && !gunDeactivated && !gameManager.GetComponent<DialogManager>().DialogueActive()) {
            gun.GetComponent<Gun>().Shoot();
        } else if (Input.GetKeyDown(KeyCode.Mouse0) && !hasGun && knifeTimer >= 0.5f && !gameManager.GetComponent<DialogManager>().DialogueActive() || 
            Input.GetKeyDown(KeyCode.Mouse0) && hasGun && knifeTimer >= 0.5f && gunDeactivated && !gameManager.GetComponent<DialogManager>().DialogueActive())
        {
            knife.KnifeEnemiesInRange(knifeDamage);
            knife.DestroyObjectsInRange();
            knifeTimer = 0f;
            swingKnife = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && hasGun && !gunDeactivated && !gameManager.GetComponent<DialogManager>().DialogueActive()) {
            transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void OnDrawGizmos() {
        if (drawPlayerGizmos) {
            Gizmos.color = Color.green;

            //Top 1
            if (Input.GetKey(KeyCode.W)) {
                Gizmos.DrawRay(transform.position - new Vector3(0.3f, 0f, 0f), Vector3.up * 0.5f);
            }
            //Top 2
            if (Input.GetKey(KeyCode.W)) {
                Gizmos.DrawRay(transform.position + new Vector3(0.3f, 0f, 0f), Vector3.up * 0.5f);
            }
            //Bottom 1
            if (Input.GetKey(KeyCode.S)) {
                Gizmos.DrawRay(transform.position - new Vector3(0.3f, 0f, 0f), Vector3.down * 0.5f);
            }
            //Bottom 2
            if (Input.GetKey(KeyCode.S)) {
                Gizmos.DrawRay(transform.position + new Vector3(0.3f, 0f, 0f), Vector3.down * 0.5f);
            }
            //Left 1
            if (Input.GetKey(KeyCode.A)) {
                Gizmos.DrawRay(transform.position - new Vector3(0f, 0.3f, 0f), Vector3.left * 0.5f);
            }
            //Left 2
            if (Input.GetKey(KeyCode.A)) {
                Gizmos.DrawRay(transform.position + new Vector3(0f, 0.3f, 0f), Vector3.left * 0.5f);
            }
            //Right 1
            if (Input.GetKey(KeyCode.D)) {
                Gizmos.DrawRay(transform.position - new Vector3(0f, 0.3f, 0f), Vector3.right * 0.5f);
            }
            //Right 2
            if (Input.GetKey(KeyCode.D)) {
                Gizmos.DrawRay(transform.position + new Vector3(0f, 0.3f, 0f), Vector3.right * 0.5f);
            }


        }
    }
}
