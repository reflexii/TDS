using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour {

    public enum Weapons { Pistol, Shotgun, SMG, Rifle };
    public Weapons weaponInUse;
    public GameObject bulletPrefab;
    public GameObject bulletPrefabBig;
    public Transform bulletSpawnPoint;
    public bool playerOwned = true;

    public float pistolShootCooldown = 0.2f;
    public float shotgunShootCooldown = 1f;
    public float smgShootCooldown = 0.04f;
    public float rifleShootCooldown = 0.08f;

    public float pistolDamage;
    public float shotgunDamage;
    public float smgDamage;
    public float rifleDamage;

    public int pistolMagazineSize = 7;
    public int shotgunMagazineSize = 2;
    public int smgMagazineSize = 30;
    public int rifleMagazineSize = 20;
    public int currentMagazineSize;
    public bool gunOnTheFloor = false;

    //create own sprites for grounded weapons!
    public Sprite pistolImage;
    public Sprite shotgunImage;
    public Sprite smgImage;
    public Sprite rifleImage;

    public Image currentGunImage;
    public Text magazineText;

    private GameManager gameManager;
    private ObjectPooler pool;
    private Vector3 shootingDirection = Vector3.zero;
    private Vector3 shotgunRandomSpread;
    private Vector3 smgRandomSpread;
    private float runningCooldown = 0f;
    private int maxMagazineSize;
    private SpriteRenderer sr;

    //throw weapon after switch
    public bool throwWeapon = false;
    public Vector3 throwDirection;
    public float throwSpeed;
    private float originalThrowSpeed;

    private Camera mainCamera;
    private GameObject player;

    //line
    private LineRenderer lineRenderer;


    void Awake () {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pool = gameManager.GetComponent<ObjectPooler>();
        magazineText = GameObject.Find("MagazineText").GetComponent<Text>();
        currentGunImage = GameObject.Find("GunImage").GetComponent<Image>();
        sr = GetComponent<SpriteRenderer>();
        lineRenderer = transform.Find("ShootingLine").gameObject.GetComponent<LineRenderer>();
        Vector3[] initLaserPositions = new Vector3[2] { transform.position, transform.position };
        lineRenderer.SetPositions(initLaserPositions);
        originalThrowSpeed = throwSpeed;
        WeaponPreparation();

        if (playerOwned)
        {
            SetUIImage();
        }
    }

    void ShootLaser(Vector3 from, Vector3 dir, float distance) {
        RaycastHit2D hit = Physics2D.Raycast(from, dir, distance, 1 << LayerMask.NameToLayer("Wall"));
        Vector3 endPos = from + (distance * dir);

        if (hit.collider != null) {
            endPos = hit.point;
        }
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, endPos);
    }

    void WeaponThrow()
    {
        if (throwWeapon)
        {
            gameObject.transform.position += throwDirection * Time.deltaTime * throwSpeed;
            throwSpeed -= Time.deltaTime * 35f;

            if (throwSpeed < 0f)
            {
                throwWeapon = false;
                throwSpeed = originalThrowSpeed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !playerOwned && gunOnTheFloor)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, throwDirection, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
            if (hit)
            {
                Vector3 hitNormal = hit.normal;
                hitNormal = hit.transform.TransformDirection(hitNormal);

                if (hitNormal == hit.transform.up)
                {
                    throwDirection = Vector3.Reflect(throwDirection, Vector3.up);
                } else if (hitNormal == -hit.transform.up)
                {
                    throwDirection = Vector3.Reflect(throwDirection, Vector3.down);
                } else if (hitNormal == -hit.transform.right)
                {
                    throwDirection = Vector3.Reflect(throwDirection, -Vector3.right);
                }
                else if (hitNormal == hit.transform.right)
                {
                    throwDirection = Vector3.Reflect(throwDirection, Vector3.right);
                }

            }
        } else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            GetComponent<Collider2D>().enabled = false;
        }
    }

    void Update () {

        runningCooldown += Time.deltaTime;
        //UI update
        if (playerOwned)
        {
            SetUIImage();
            lineRenderer.enabled = true;
            //Laser
            // convert mouse position into world coordinates
            Vector2 mouseScreenPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // get direction you want to point at
            Vector2 direction = (mouseScreenPosition - (Vector2)transform.position).normalized;

            float dist = Vector3.Distance(bulletSpawnPoint.transform.position, mouseScreenPosition);

            ShootLaser(bulletSpawnPoint.transform.position, direction, dist);
        } else {
            lineRenderer.enabled = false;
        }

        UpdateSprite();
        WeaponThrow();

    }

    void UpdateSprite()
    {
        if (gunOnTheFloor)
        {
            sr.enabled = true;
            switch(weaponInUse)
            {
                case Weapons.Pistol:
                    sr.sprite = pistolImage;
                    break;
                case Weapons.Shotgun:
                    sr.sprite = shotgunImage;
                    break;
                case Weapons.SMG:
                    sr.sprite = smgImage;
                    break;
                case Weapons.Rifle:
                    sr.sprite = rifleImage;
                    break;
            }
        } else
        {
            sr.enabled = false;
        }
    }

    public void Shoot() {
        if (!gunOnTheFloor)
        {

            // convert mouse position into world coordinates
            Vector2 mouseScreenPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // get direction you want to point at
            if (playerOwned)
            {
                shootingDirection = (mouseScreenPosition - (Vector2)player.transform.position).normalized;
            }
            else
            {
                shootingDirection = (player.transform.position - transform.parent.position).normalized;
            }

            switch (weaponInUse)
            {
                case Weapons.Pistol:
                    if (runningCooldown > pistolShootCooldown && currentMagazineSize > 0)
                    {
                        transform.parent.transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = true;

                        GameObject pistolBullet = pool.GetPooledBullet();
                        pistolBullet.SetActive(true);
                        pistolBullet.transform.position = bulletSpawnPoint.position;
                        pistolBullet.GetComponent<Bullet>().direction = shootingDirection;
                        //rotate
                        float angle = Mathf.Atan2(pistolBullet.GetComponent<Bullet>().direction.y, pistolBullet.GetComponent<Bullet>().direction.x) * Mathf.Rad2Deg;
                        pistolBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                        pistolBullet.GetComponent<Bullet>().bulletSpeed = 28f;
                        pistolBullet.GetComponent<Bullet>().bulletDamage = pistolDamage;
                        runningCooldown = 0f;
                        currentMagazineSize--;

                        if (!playerOwned)
                        {
                            pistolBullet.GetComponent<Bullet>().playerBullet = false;
                        } else {
                            pistolBullet.GetComponent<Bullet>().playerBullet = true;
                        }
                    } else {
                        transform.parent.transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = false;
                    }
                    break;
                case Weapons.Shotgun:
                    if (runningCooldown > shotgunShootCooldown && currentMagazineSize > 0)
                    {
                        transform.parent.transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = true;
                       
                        for (int i = 0; i < 6; i++)
                        {
                            GameObject shotgunPellet = pool.GetPooledBigBullet();
                            shotgunPellet.SetActive(true);
                            shotgunPellet.transform.position = bulletSpawnPoint.position;
                            RandomizeShotgunDirection();
                            shotgunPellet.GetComponent<Bullet>().direction = shootingDirection + shotgunRandomSpread;
                            //rotate
                            float angle = Mathf.Atan2(shotgunPellet.GetComponent<Bullet>().direction.y, shotgunPellet.GetComponent<Bullet>().direction.x) * Mathf.Rad2Deg;
                            shotgunPellet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                            float randomizeSpeed = Random.Range(2f, 10f);
                            shotgunPellet.GetComponent<Bullet>().bulletSpeed = 22f + randomizeSpeed;
                            shotgunPellet.GetComponent<Bullet>().bulletDamage = shotgunDamage;
                            runningCooldown = 0f;

                            if (!playerOwned)
                            {
                                shotgunPellet.GetComponent<Bullet>().playerBullet = false;
                            } else {
                                shotgunPellet.GetComponent<Bullet>().playerBullet = true;
                            }
                        }

                        currentMagazineSize--;
                    } else {
                        transform.parent.transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = false;
                    }
                    break;
                case Weapons.SMG:
                    if (runningCooldown > smgShootCooldown && currentMagazineSize > 0)
                    {
                        transform.parent.transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = true;
                        
                        GameObject smgBullet = pool.GetPooledBullet();
                        smgBullet.SetActive(true);
                        smgBullet.transform.position = bulletSpawnPoint.position;
                        RandomizeSMGDirection();
                        smgBullet.GetComponent<Bullet>().direction = shootingDirection + smgRandomSpread;
                        //rotate
                        float angle = Mathf.Atan2(smgBullet.GetComponent<Bullet>().direction.y, smgBullet.GetComponent<Bullet>().direction.x) * Mathf.Rad2Deg;
                        smgBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                        smgBullet.GetComponent<Bullet>().bulletSpeed = 28f;
                        smgBullet.GetComponent<Bullet>().bulletDamage = smgDamage;
                        runningCooldown = 0f;
                        currentMagazineSize--;

                        if (!playerOwned)
                        {
                            smgBullet.GetComponent<Bullet>().playerBullet = false;
                        } else {
                            smgBullet.GetComponent<Bullet>().playerBullet = true;
                        }
                    } else {
                        transform.parent.transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = false;
                    }
                    break;
                case Weapons.Rifle:
                    if (runningCooldown > rifleShootCooldown && currentMagazineSize > 0)
                    {
                        transform.parent.transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = true;
                       
                        GameObject rifleBullet = pool.GetPooledBigBullet();
                        rifleBullet.SetActive(true);
                        rifleBullet.transform.position = bulletSpawnPoint.position;
                        rifleBullet.GetComponent<Bullet>().direction = shootingDirection;

                        //rotate
                        float angle = Mathf.Atan2(rifleBullet.GetComponent<Bullet>().direction.y, rifleBullet.GetComponent<Bullet>().direction.x) * Mathf.Rad2Deg;
                        rifleBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                        rifleBullet.GetComponent<Bullet>().bulletSpeed = 32f;
                        rifleBullet.GetComponent<Bullet>().bulletDamage = rifleDamage;
                        runningCooldown = 0f;
                        currentMagazineSize--;

                        if (!playerOwned)
                        {
                            rifleBullet.GetComponent<Bullet>().playerBullet = false;
                        } else {
                            rifleBullet.GetComponent<Bullet>().playerBullet = true;
                        }
                    } else {
                        transform.parent.transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = false;
                    }
                    break;
            }
        }
    }

    public void WeaponPreparation()
    {
        switch (weaponInUse)
        {
            case Weapons.Pistol:
                currentMagazineSize = pistolMagazineSize;
                GetComponent<BoxCollider2D>().offset = new Vector2(0.1f, -0.06f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.93f, 0.69f);

                if (!gunOnTheFloor) {
                    if (playerOwned) {
                        bulletSpawnPoint.localPosition = new Vector3(0.916f, -0.08f, bulletSpawnPoint.localPosition.z);
                    } else {
                        bulletSpawnPoint.localPosition = new Vector3(0.089f, 1.001f, bulletSpawnPoint.localPosition.z);
                    }
                }
                break;
            case Weapons.Shotgun:
                currentMagazineSize = shotgunMagazineSize;
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.05f, -0.07f);
                GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.69f);
                if (!gunOnTheFloor) {
                    if (playerOwned) {
                        bulletSpawnPoint.localPosition = new Vector3(1.205f, -0.286f, bulletSpawnPoint.localPosition.z);
                    } else {
                        bulletSpawnPoint.localPosition = new Vector3(0.2502f, 1.001f, bulletSpawnPoint.localPosition.z);
                    }
                }
                break;
            case Weapons.SMG:
                currentMagazineSize = smgMagazineSize;
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.01f, -0.03f);
                GetComponent<BoxCollider2D>().size = new Vector2(1.62f, 0.81f);
                if (!gunOnTheFloor) {
                    if (playerOwned) {
                        bulletSpawnPoint.localPosition = new Vector3(0.875f, -0.297f, bulletSpawnPoint.localPosition.z);
                    } else {
                        bulletSpawnPoint.localPosition = new Vector3(0.253f, 0.904f, bulletSpawnPoint.localPosition.z);
                    }
                }
                break;
            case Weapons.Rifle:
                currentMagazineSize = rifleMagazineSize;
                GetComponent<BoxCollider2D>().offset = new Vector2(0.38f, -0.03f);
                GetComponent<BoxCollider2D>().size = new Vector2(2.24f, 0.93f);
                if (!gunOnTheFloor) {
                    if (playerOwned) {
                        bulletSpawnPoint.localPosition = new Vector3(1.205f, -0.286f, bulletSpawnPoint.localPosition.z);
                    } else {
                        bulletSpawnPoint.localPosition = new Vector3(0.2503f, 1.3389f, bulletSpawnPoint.localPosition.z);
                    }
                }
                break;
        }
        maxMagazineSize = currentMagazineSize;
    }

    public void SetUIImage()
    {
            switch (weaponInUse)
            {
                case Weapons.Pistol:
                    currentGunImage.sprite = pistolImage;
                    break;
                case Weapons.Shotgun:
                    currentGunImage.sprite = shotgunImage;
                    break;
                case Weapons.SMG:
                    currentGunImage.sprite = smgImage;
                    break;
                case Weapons.Rifle:
                    currentGunImage.sprite = rifleImage;
                    break;
            }

            magazineText.text = currentMagazineSize + "/" + maxMagazineSize;
        }

    private void RandomizeShotgunDirection() {
        float x = Random.Range(-0.2f, 0.2f);
        float y = Random.Range(-0.2f, 0.2f);
        shotgunRandomSpread = new Vector3(x, y, 0f);
    }

    private void RandomizeSMGDirection()
    {
        float x = Random.Range(-0.12f, 0.12f);
        float y = Random.Range(-0.12f, 0.12f);
        smgRandomSpread = new Vector3(x, y, 0f);
    }
}
