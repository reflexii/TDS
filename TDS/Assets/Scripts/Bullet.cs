using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float bulletSpeed;
    public Vector3 direction;
    public bool playerBullet = true;
    public float bulletExistTime = 2f;
    public float bulletDamage;

    private float runningDeathTime;

    private void Start()
    {
        if (playerBullet)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
        } else
        {
            gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
        }
    }

    private void OnEnable() {
        runningDeathTime = 0f;
    }

    void Update () {
        runningDeathTime += Time.deltaTime;
        transform.position += direction.normalized * bulletSpeed * Time.deltaTime;

        if (runningDeathTime >= bulletExistTime)
        {
            gameObject.SetActive(false);
        }

	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            gameObject.SetActive(false);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && collision.gameObject.tag == "Destructable" && collision.gameObject.GetComponent<DestroyableObject>() != null) {
            if (collision.gameObject.GetComponent<DestroyableObject>().breakableWithBullets) {
                gameObject.SetActive(false);
                collision.gameObject.GetComponent<DestroyableObject>().DestroyWall();
            }
        }

        if (playerBullet && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (collision.gameObject.GetComponent<MovingEnemy>().enemyHealth > 0) {
                collision.gameObject.GetComponent<MovingEnemy>().DamageEnemy(bulletDamage);
                gameObject.SetActive(false);
            }

        }
        if (!playerBullet && collision.gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            //Player loses hp + death
        }
    }
}
