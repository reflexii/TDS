using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour {

    public List<GameObject> knifableEnemies;
    public List<GameObject> destroyableObjects;

    private void Awake()
    {
        knifableEnemies = new List<GameObject>();
        destroyableObjects = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            knifableEnemies.Add(collision.gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && collision.gameObject.tag == "Destructable" && collision.gameObject.GetComponent<DestroyableObject>() != null) {
            if (collision.gameObject.GetComponent<DestroyableObject>().breakableWithBullets) {
                destroyableObjects.Add(collision.gameObject);
                Debug.Log("Glass added");
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Backstab")) {
            collision.transform.parent.gameObject.GetComponent<MovingEnemy>().backstabbable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            knifableEnemies.Remove(collision.gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && collision.gameObject.tag == "Destructable" && collision.gameObject.GetComponent<DestroyableObject>() != null) {
            if (collision.gameObject.GetComponent<DestroyableObject>().breakableWithBullets) {
                destroyableObjects.Remove(collision.gameObject);
                Debug.Log("Glass removed");
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Backstab")) {
            collision.transform.parent.gameObject.GetComponent<MovingEnemy>().backstabbable = false;
        }
    }

    public void DestroyObjectsInRange() {
        if (destroyableObjects != null) {
            if (destroyableObjects.Count >= 1) {
                for (int i = 0; i < destroyableObjects.Count; i++) {
                    Debug.Log(destroyableObjects.Count);
                    destroyableObjects[i].GetComponent<DestroyableObject>().DestroyWall();
                }
            }
        }
    }

    public void KnifeEnemiesInRange(float damageValue)
    {

        //damage enemies in range, if on backstab range -> double damage
        for (int i = 0; i < knifableEnemies.Count; i++)
        {
            if (knifableEnemies[i] != null)
            {
                if (!knifableEnemies[i].GetComponent<MovingEnemy>().backstabbable) {
                    knifableEnemies[i].GetComponent<MovingEnemy>().DamageEnemy(damageValue);
                } else {
                    knifableEnemies[i].GetComponent<MovingEnemy>().DamageEnemy(damageValue*2f);
                }
                
            }
            
        }
    }
}
