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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            knifableEnemies.Remove(collision.gameObject);
        }
    }

    public void KnifeEnemiesInRange(float damageValue)
    {
        for (int i = 0; i < knifableEnemies.Count; i++)
        {
            if (knifableEnemies[i] != null)
            {
                knifableEnemies[i].GetComponent<MovingEnemy>().DamageEnemy(damageValue);
            }
            
        }
    }
}
