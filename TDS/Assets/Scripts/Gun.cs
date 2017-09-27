using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public enum Weapons { Melee, Pistol, Shotgun, SMG, Rifle};
    public Weapons weaponInUse;
    public GameObject bulletPrefab;
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

    private GameManager gameManager;
    private Vector3 shootingDirection = Vector3.zero;
    private Vector3 shotgunRandomSpread;
    private Vector3 smgRandomSpread;
    private float runningCooldown = 0f;

	void Awake () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        WeaponPreparation();
	}
	
	void Update () {
        
        runningCooldown += Time.deltaTime;
    }

    public void Shoot() {

        // convert mouse position into world coordinates
        Vector2 mouseScreenPosition = gameManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        if (playerOwned)
        {
            shootingDirection = (mouseScreenPosition - (Vector2)gameManager.Player.transform.position).normalized;
        }
        else
        {
            shootingDirection = (gameManager.Player.transform.position - transform.parent.position).normalized;
        }

        switch (weaponInUse) {
            case Weapons.Melee:
                break;
            case Weapons.Pistol:
                if (runningCooldown > pistolShootCooldown && currentMagazineSize > 0) {
                    GameObject pistolBullet = Instantiate<GameObject>(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                    pistolBullet.GetComponent<Bullet>().direction = shootingDirection;
                    pistolBullet.GetComponent<Bullet>().bulletSpeed = 10f;
                    pistolBullet.GetComponent<Bullet>().bulletDamage = pistolDamage;
                    runningCooldown = 0f;
                    currentMagazineSize--;

                    if (!playerOwned)
                    {
                        pistolBullet.GetComponent<Bullet>().playerBullet = false;
                    }
                }
                break;
            case Weapons.Shotgun:
                if (runningCooldown > shotgunShootCooldown && currentMagazineSize > 0) {
                    for (int i = 0; i < 8; i++) {
                        GameObject shotgunPellet = Instantiate<GameObject>(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                        RandomizeShotgunDirection();
                        shotgunPellet.GetComponent<Bullet>().direction = shootingDirection + shotgunRandomSpread;
                        float randomizeSpeed = Random.Range(-10f, 10f);
                        shotgunPellet.GetComponent<Bullet>().bulletSpeed = 18f + randomizeSpeed;
                        shotgunPellet.GetComponent<Bullet>().bulletDamage = shotgunDamage;
                        runningCooldown = 0f;

                        if (!playerOwned)
                        {
                            shotgunPellet.GetComponent<Bullet>().playerBullet = false;
                        }
                    }

                    currentMagazineSize--;
                }
                break;
            case Weapons.SMG:
                if (runningCooldown > smgShootCooldown && currentMagazineSize > 0)
                {
                    GameObject smgBullet = Instantiate<GameObject>(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                    RandomizeSMGDirection();
                    smgBullet.GetComponent<Bullet>().direction = shootingDirection + smgRandomSpread;
                    smgBullet.GetComponent<Bullet>().bulletSpeed = 17f;
                    smgBullet.GetComponent<Bullet>().bulletDamage = smgDamage;
                    runningCooldown = 0f;
                    currentMagazineSize--;

                    if (!playerOwned)
                    {
                        smgBullet.GetComponent<Bullet>().playerBullet = false;
                    }
                }
                break;
            case Weapons.Rifle:
                if (runningCooldown > rifleShootCooldown && currentMagazineSize > 0)
                {
                    GameObject rifleBullet = Instantiate<GameObject>(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                    rifleBullet.GetComponent<Bullet>().direction = shootingDirection;
                    rifleBullet.GetComponent<Bullet>().bulletSpeed = 20f;
                    rifleBullet.GetComponent<Bullet>().bulletDamage = rifleDamage;
                    runningCooldown = 0f;
                    currentMagazineSize--;

                    if (!playerOwned)
                    {
                        rifleBullet.GetComponent<Bullet>().playerBullet = false;
                    }
                }
                
                break;
        }
    }

    public void WeaponPreparation()
    {
        switch (weaponInUse)
        {
            case Weapons.Melee:
                break;
            case Weapons.Pistol:
                currentMagazineSize = pistolMagazineSize;
                break;
            case Weapons.Shotgun:
                currentMagazineSize = shotgunMagazineSize;
                break;
            case Weapons.SMG:
                currentMagazineSize = smgMagazineSize;
                break;
            case Weapons.Rifle:
                currentMagazineSize = rifleMagazineSize;
                break;
        }
    }

    private void RandomizeShotgunDirection() {
        float x = Random.Range(-0.12f, 0.12f);
        float y = Random.Range(-0.12f, 0.12f);
        shotgunRandomSpread = new Vector3(x, y, 0f);
    }

    private void RandomizeSMGDirection()
    {
        float x = Random.Range(-0.2f, 0.2f);
        float y = Random.Range(-0.2f, 0.2f);
        smgRandomSpread = new Vector3(x, y, 0f);
    }
}
