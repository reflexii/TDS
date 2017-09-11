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

    private GameManager gameManager;
    private Vector3 shootingDirection = Vector3.zero;
    private Vector3 shotgunRandomSpread;
    private float runningCooldown = 0f;

	void Awake () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	void Update () {
        // convert mouse position into world coordinates
        Vector2 mouseScreenPosition = gameManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        if (playerOwned) {
            shootingDirection = (mouseScreenPosition - (Vector2)gameManager.Player.transform.position).normalized;
        } else {
            shootingDirection = gameManager.Player.transform.position - transform.parent.position;
        }
        

        runningCooldown += Time.deltaTime;
    }

    public void Shoot() {
        switch (weaponInUse) {
            case Weapons.Melee:
                break;
            case Weapons.Pistol:
                if (runningCooldown > pistolShootCooldown) {
                    GameObject pistolBullet = Instantiate<GameObject>(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                    pistolBullet.GetComponent<Bullet>().direction = shootingDirection;
                    pistolBullet.GetComponent<Bullet>().bulletSpeed = 25f;
                    runningCooldown = 0f;
                }
                break;
            case Weapons.Shotgun:
                if (runningCooldown > shotgunShootCooldown) {
                    for (int i = 0; i < 8; i++) {
                        GameObject shotgunPellet = Instantiate<GameObject>(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                        RandomizeShotgunDirection();
                        shotgunPellet.GetComponent<Bullet>().direction = shootingDirection + shotgunRandomSpread;
                        shotgunPellet.GetComponent<Bullet>().bulletSpeed = 40f;
                        runningCooldown = 0f;
                    }
                }
                break;
            case Weapons.SMG:
                break;
            case Weapons.Rifle:
                break;
        }
    }

    private void RandomizeShotgunDirection() {
        float x = Random.Range(-0.12f, 0.12f);
        float y = Random.Range(-0.12f, 0.12f);
        shotgunRandomSpread = new Vector3(x, y, 0f);
    }
}
