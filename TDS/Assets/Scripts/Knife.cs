using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour {

    public List<GameObject> knifableEnemies;

    private void Awake()
    {
        knifableEnemies = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            knifableEnemies.Add(collision.gameObject);
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

        if (collision.gameObject.layer == LayerMask.NameToLayer("Backstab")) {
            collision.transform.parent.gameObject.GetComponent<MovingEnemy>().backstabbable = false;
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
