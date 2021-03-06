﻿using System.Collections;
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && collision.transform.tag != "SeeThroughItem") {
            gameObject.SetActive(false);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && collision.gameObject.tag == "Destructable" && collision.gameObject.GetComponent<DestroyableObject>() != null ||
            collision.gameObject.layer == LayerMask.NameToLayer("Wall") && collision.gameObject.tag == "SeeThroughDestructable" && collision.gameObject.GetComponent<DestroyableObject>() != null) {
            if (collision.gameObject.GetComponent<DestroyableObject>().breakableWithBullets) {
                if (collision.gameObject.GetComponent<DestroyableObject>().onlyWithBossBullets && !playerBullet) {
                    gameObject.SetActive(false);
                    collision.gameObject.GetComponent<DestroyableObject>().TakeDamage(1f);
                } else if (!collision.gameObject.GetComponent<DestroyableObject>().onlyWithBossBullets) {
                    gameObject.SetActive(false);
                    collision.gameObject.GetComponent<DestroyableObject>().TakeDamage(bulletDamage);
                }
                
            }
        }

        if (playerBullet && collision.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
            playerBullet && collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            if (collision.gameObject.GetComponent<MovingEnemy>() != null) {
                if (collision.gameObject.GetComponent<MovingEnemy>().enemyHealth > 0) {
                    collision.gameObject.GetComponent<MovingEnemy>().DamageEnemy(bulletDamage);
                    gameObject.SetActive(false);
                }
            } else if (collision.gameObject.GetComponent<Scientist>() != null) {
                if (collision.gameObject.GetComponent<Scientist>().enemyHealth > 0) {
                    collision.gameObject.GetComponent<Scientist>().DamageEnemy(bulletDamage);
                    gameObject.SetActive(false);
                }
            } else if (collision.gameObject.GetComponent<Boss>() != null) {
                if (collision.gameObject.GetComponent<Boss>().enemyHealth > 0) {
                    collision.gameObject.GetComponent<Boss>().DamageEnemy(bulletDamage);
                    gameObject.SetActive(false);
                }
            }
        }
        if (!playerBullet && collision.gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            if (collision.gameObject.GetComponent<PlayerMovement>().playerCurrentHealth > 0) {
                collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(bulletDamage);
                gameObject.SetActive(false);
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("VIPDamage")) {
            if (collision.gameObject.transform.parent.GetComponent<VIP>().vipCurrentHealth > 0) {
                collision.gameObject.transform.parent.GetComponent<VIP>().TakeDamage(bulletDamage);
                gameObject.SetActive(false);
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBlocker") && collision.gameObject.tag == "Button" && !playerBullet) {
            GameObject buttonSprite = collision.transform.parent.transform.Find("ButtonSprite").gameObject;

            if (buttonSprite.GetComponent<Button2>().breakableWithEnemyBullets) {
                buttonSprite.GetComponent<Button2>().broken = true;
                buttonSprite.GetComponent<Button2>().ToggleOff();
                buttonSprite.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
}
