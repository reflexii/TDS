using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour {

    public GameObject ps;
    public bool breakableWithBullets = false;
    public float health = 1f;

    private float currentHealth;

    public void Awake() {
        currentHealth = health;
    }

    public void DestroyWall()
    {
        Instantiate<GameObject>(ps, transform.position, Quaternion.identity);
        Destroy(gameObject, 0f);
        //play animation
        //play sound
    }

    public void TakeDamage(float value) {
        currentHealth -= value;

        if (currentHealth <= 0) {
            DestroyWall();
        }
    }
}
