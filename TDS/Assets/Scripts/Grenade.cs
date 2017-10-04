using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

    public float grenadeSpeed;
    public Vector3 grenadeMoveDirection = Vector3.up;
    public float grenadeVelocityReductionSpeed = 5f;
    public float timeBeforeGrenadeExplosion = 4f;
    public float grenadeDamage = 5;

    private float runningGrenadeTime = 0f;
	
	void Update () {
        runningGrenadeTime += Time.deltaTime;
        grenadeSpeed -= Time.deltaTime * grenadeVelocityReductionSpeed;

        if (grenadeSpeed <= 0f)
        {
            grenadeSpeed = 0f;
        }

        gameObject.transform.position += grenadeMoveDirection.normalized * Time.deltaTime * grenadeSpeed;

        if (runningGrenadeTime >= timeBeforeGrenadeExplosion)
        {
            ExplodeGrenade(); 
        }
        
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            ExplodeGrenade();
        }
    }

    public void ExplodeGrenade()
    {
        //Explode destroyable walls
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (colliders != null)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null && colliders[i].gameObject.tag == "Destructable")
                {
                    colliders[i].gameObject.GetComponent<DestroyableObject>().DestroyWall();
                }
            }
        }

        //Explode enemies, in three different radiuses, when enemy is close he takes damage from all three etc
        Collider2D[] firstExplosionRadius = Physics2D.OverlapCircleAll(transform.position, 1.7f, 1 << LayerMask.NameToLayer("Enemy"));
        Collider2D[] secondExplosionRadius = Physics2D.OverlapCircleAll(transform.position, 3f, 1 << LayerMask.NameToLayer("Enemy"));

        if (firstExplosionRadius != null)
        {
            for (int i = 0; i < firstExplosionRadius.Length; i++)
            {
                if (firstExplosionRadius[i] != null)
                {
                    firstExplosionRadius[i].gameObject.GetComponent<MovingEnemy>().DamageEnemy(grenadeDamage);
                    Debug.Log("First");
                }
            }
        }

        if (secondExplosionRadius != null)
        {
            for (int i = 0; i < secondExplosionRadius.Length; i++)
            {
                if (secondExplosionRadius[i] != null)
                {
                    secondExplosionRadius[i].gameObject.GetComponent<MovingEnemy>().DamageEnemy(grenadeDamage);
                    Debug.Log("Second");
                }
            }
        }

        //play sound
        //play animation
        Destroy(gameObject, 0f);
    }
}
