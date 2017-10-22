using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

    public float grenadeSpeed;
    public Vector3 grenadeMoveDirection = Vector3.up;
    public float grenadeVelocityReductionSpeed = 5f;
    public float timeBeforeGrenadeExplosion = 4f;
    public float grenadeDamage = 5;
    public GameObject explosionPrefab;

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

        //Explode enemies, in two different radiuses, when enemy is close he takes damage from all two
        Collider2D[] firstExplosionRadius = Physics2D.OverlapCircleAll(transform.position, 1.7f, 1 << LayerMask.NameToLayer("Enemy"));
        Collider2D[] secondExplosionRadius = Physics2D.OverlapCircleAll(transform.position, 3f, 1 << LayerMask.NameToLayer("Enemy"));

        if (firstExplosionRadius != null)
        {
            for (int i = 0; i < firstExplosionRadius.Length; i++)
            {
                if (firstExplosionRadius[i] != null)
                {
                    if (!Physics2D.Raycast(transform.position, firstExplosionRadius[i].transform.position - transform.position, Vector3.Distance(transform.position, firstExplosionRadius[i].transform.position), 1 << LayerMask.NameToLayer("Wall")))
                    {
                        firstExplosionRadius[i].gameObject.GetComponent<MovingEnemy>().DamageEnemy(grenadeDamage);
                    }
                }
            }
        }

        if (secondExplosionRadius != null)
        {
            for (int i = 0; i < secondExplosionRadius.Length; i++)
            {
                if (secondExplosionRadius[i] != null)
                {
                    if (!Physics2D.Raycast(transform.position, secondExplosionRadius[i].transform.position - transform.position, Vector3.Distance(transform.position, secondExplosionRadius[i].transform.position), 1 << LayerMask.NameToLayer("Wall")))
                    {
                        secondExplosionRadius[i].gameObject.GetComponent<MovingEnemy>().DamageEnemy(grenadeDamage);
                    }
                }
            }
        }

        Collider2D[] explosionSoundWave = Physics2D.OverlapCircleAll(transform.position, 10f, 1 << LayerMask.NameToLayer("Enemy"));

        if (explosionSoundWave != null)
        {
            for (int i = 0; i < explosionSoundWave.Length; i++)
            {
                if (explosionSoundWave[i] != null)
                {
                    
                    if (explosionSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.Moving ||
                        explosionSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.WaitingToMove ||
                        explosionSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.SearchingPlayer &&
                        explosionSoundWave[i].GetComponent<MovingEnemy>().GetRunningTime() >= 3f)
                    {
                        explosionSoundWave[i].GetComponent<MovingEnemy>().playerSearchPosition = transform.position;
                        explosionSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing = MovingEnemy.CurrentStance.SearchingPlayer;
                    }
                    
                }
            }
        }

        //play sound
        Instantiate<GameObject>(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, 0f);
    }
}
