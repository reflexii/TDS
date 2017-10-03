﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    private GameManager gameManager;
    public float movementSpeed;
    public bool drawPlayerGizmos = true;

    //Raycasting and movement
    private bool disableTop = false;
    private bool disableBottom = false;
    private bool disableLeft = false;
    private bool disableRight = false;

    //Gun
    public GameObject gun;

    //Grenade
    private float howFastGrenadeSpeedIncreases = 3f;
    private float increasedGrenadeSpeed = 0f;
    public GameObject grenadePrefab;

    //Health
    public float playerHealth = 100f;

    //Gunswitching
    public bool gunInRange = false;
    private GameObject floorGun;
    public bool hasGun;

    //hud sprites
    public Sprite knifeSprite;
    public Image currentGunImage;
    public Text magazineText;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        magazineText = GameObject.Find("MagazineText").GetComponent<Text>();
        currentGunImage = GameObject.Find("GunImage").GetComponent<Image>();

        if (hasGun)
        {
            gun = transform.Find("Gun").gameObject;
        } else
        {
            Destroy(transform.Find("Gun").gameObject, 0f);
        }
    }

    void Update () {
        Movement();
        MouseLook();
        WallCheck();
        Shooting();
        Grenade();
        GunSwitching();
        KnifeSprite();
	}

    void KnifeSprite()
    {
        if (!hasGun)
        {
            currentGunImage.sprite = knifeSprite;
            magazineText.text = "";
        }
    }

    void Movement()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !disableTop && !disableRight) {
            transform.position += new Vector3(1f, 1f).normalized * movementSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !disableTop && !disableLeft) {
            transform.position += new Vector3(-1f, 1f).normalized * movementSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !disableBottom && !disableRight) {
            transform.position += new Vector3(1f, -1f).normalized * movementSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && !disableBottom && !disableLeft) {
            transform.position += new Vector3(-1f, -1f).normalized * movementSpeed * Time.deltaTime;
        } else {
            if (Input.GetKey(KeyCode.W) && !disableTop) {
                transform.position += new Vector3(0f, 1f) * movementSpeed * Time.deltaTime;
            }
             else if (Input.GetKey(KeyCode.S) && !disableBottom) {
                transform.position += new Vector3(0f, -1f) * movementSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.A) && !disableLeft) {
                transform.position += new Vector3(-1f, 0f) * movementSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D) && !disableRight) {
                transform.position += new Vector3(1f, 0f) * movementSpeed * Time.deltaTime;
            }
        }
    }

    void Grenade()
    {
        if (Input.GetKey(KeyCode.E))
        {
            increasedGrenadeSpeed += Time.deltaTime * howFastGrenadeSpeedIncreases;
            if (increasedGrenadeSpeed >= 6f)
            {
                increasedGrenadeSpeed = 6f;
            }
        }
        if (Input.GetKeyUp(KeyCode.E))
        {

            // convert mouse position into world coordinates
            Vector2 mouseScreenPosition = gameManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // get direction you want to point at
            Vector2 direction = (mouseScreenPosition - (Vector2)transform.position).normalized;

            GameObject gre = Instantiate<GameObject>(grenadePrefab, gameObject.transform.position, Quaternion.identity);
            Grenade g = gre.GetComponent<Grenade>();
            g.grenadeMoveDirection = direction;
            g.grenadeSpeed += increasedGrenadeSpeed;

            increasedGrenadeSpeed = 0f;
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
    }

    void DropGun()
    {
        // convert mouse position into world coordinates
        Vector2 mouseScreenPosition = gameManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        gun.GetComponent<Gun>().throwDirection = (mouseScreenPosition - (Vector2)gameManager.Player.transform.position).normalized;
        gun.GetComponent<Gun>().throwWeapon = true;

        gun.transform.parent = null;
        gun.GetComponent<Gun>().playerOwned = false;
        gun.GetComponent<Gun>().gunOnTheFloor = true;
        gun.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gun = null;
        gunInRange = false;
        hasGun = false;
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
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Gun") && collision.gameObject.GetComponent<Gun>().gunOnTheFloor == true)
        {
            gunInRange = true;
            floorGun = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Gun") && collision.gameObject.GetComponent<Gun>().gunOnTheFloor == true)
        {
            gunInRange = false;
        }
    }

    void MouseLook() {
        // convert mouse position into world coordinates
        Vector2 mouseScreenPosition = gameManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        Vector2 direction = (mouseScreenPosition - (Vector2)transform.position).normalized;

        // set vector of transform directly
        transform.right = direction;
    }

    void WallCheck() {
        if (Input.GetKey(KeyCode.W)) {
            if (Physics2D.Raycast(transform.position, Vector3.up, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
                disableTop = true;
            } else {
                disableTop = false;
            }
        }
        if (Input.GetKey(KeyCode.S)) {
            if (Physics2D.Raycast(transform.position, Vector3.down, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
                disableBottom = true;
            } else {
                disableBottom = false;
            }
        }
        if (Input.GetKey(KeyCode.A)) {
            if (Physics2D.Raycast(transform.position, Vector3.left, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
                disableLeft = true;
            } else {
                disableLeft = false;
            }
        }    
        if (Input.GetKey(KeyCode.D)) {
            if (Physics2D.Raycast(transform.position, Vector3.right, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
                disableRight = true;
            } else {
                disableRight = false;
            }
        }
        
    }

    void Shooting() {
        if (Input.GetKey(KeyCode.Mouse0) && hasGun) {
            gun.GetComponent<Gun>().Shoot();
        } else if (Input.GetKeyDown(KeyCode.Mouse0) && !hasGun)
        {
            //knife
        }
    }

    private void OnDrawGizmos() {
        if (drawPlayerGizmos) {
            Gizmos.color = Color.green;

            //Top
            if (Input.GetKey(KeyCode.W)) {
                Gizmos.DrawRay(transform.position, Vector3.up);
            }
            
            //Bottom
            if (Input.GetKey(KeyCode.S)) {
                Gizmos.DrawRay(transform.position, Vector3.down);
            }
            //Left
            if (Input.GetKey(KeyCode.A)) {
                Gizmos.DrawRay(transform.position, Vector3.left);
            }
            //Right
            if (Input.GetKey(KeyCode.D)) {
                Gizmos.DrawRay(transform.position, Vector3.right);
            }
            
            
            
        }
    }
}
