using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public enum Weapons { Melee, Pistol, Shotgun, SMG, Rifle};
    public Weapons weaponInUse;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    private GameManager gameManager;
    private Vector3 shootingDirection = Vector3.zero;

	void Awake () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	void Update () {
        // convert mouse position into world coordinates
        Vector2 mouseScreenPosition = gameManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        shootingDirection = (mouseScreenPosition - (Vector2)gameManager.Player.transform.position).normalized;
    }

    public void Shoot() {
        switch (weaponInUse) {
            case Weapons.Melee:
                break;
            case Weapons.Pistol:
                GameObject pistolBullet = Instantiate<GameObject>(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                pistolBullet.GetComponent<Bullet>().direction = shootingDirection;
                pistolBullet.GetComponent<Bullet>().bulletSpeed = 25f;
                break;
            case Weapons.Shotgun:
                break;
            case Weapons.SMG:
                break;
            case Weapons.Rifle:
                break;
        }
    }
}
