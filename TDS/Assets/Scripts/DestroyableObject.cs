using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour {

    public GameObject ps;
    public bool breakableWithBullets = false;
    public float health = 1f;
    public enum MaterialType { Glass, Wood, Concrete };
    public MaterialType material;

    private float currentHealth;
    private GameManager gm;

    public void Awake() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        currentHealth = health;
    }

    public void DestroyWall()
    {
        if (material == MaterialType.Glass) {
            //Play glass sound
            gm.GetComponent<SoundManager>().PlaySound("GlassBreak", true, 0.7f, 1.3f);
        } else if (material == MaterialType.Wood) {
            //Play wood sound
            gm.GetComponent<SoundManager>().PlaySound("BoxBreak", true, 0.7f, 1.3f);
        } else if (material == MaterialType.Concrete) {
            //play concrete sound
        }

        Instantiate<GameObject>(ps, transform.position, Quaternion.identity);
        Destroy(gameObject, 0f);
    }

    public void TakeDamage(float value) {
        currentHealth -= value;

        if (currentHealth <= 0) {
            DestroyWall();
        }
    }
}
