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

    private Vector3 rotationVector;
    private GameManager gm;

    private float runningGrenadeTime = 0f;


    private void Awake() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        int randomized = Random.Range(0, 2);

        if (randomized == 0) {
            rotationVector = Vector3.forward;
        } else {
            rotationVector = Vector3.back;
        }
    }

    void Update () {
        runningGrenadeTime += Time.deltaTime;
        grenadeSpeed -= Time.deltaTime * grenadeVelocityReductionSpeed;

        if (grenadeSpeed <= 0f)
        {
            grenadeSpeed = 0f;
        }

        transform.Rotate(rotationVector * grenadeSpeed * Time.deltaTime * 100f);

        gameObject.transform.position += grenadeMoveDirection.normalized * Time.deltaTime * grenadeSpeed;

        if (runningGrenadeTime >= timeBeforeGrenadeExplosion)
        {
            ExplodeGrenade(); 
        }
        
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && collision.transform.tag != "SeeThroughItem") {
            ExplodeGrenade();
        }
    }

    public void ExplodeGrenade()
    {
        gm.GetComponent<SoundManager>().PlaySound("Grenade_free", true);
        //Explode destroyable walls
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (colliders != null)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                {
                    if (colliders[i].gameObject.tag == "Destructable" || colliders[i].gameObject.tag == "SeeThroughDestructable")
                    colliders[i].gameObject.GetComponent<DestroyableObject>().TakeDamage(5f);
                }
            }
        }

        //Explode enemies, in two different radiuses, when enemy is close he takes damage from both
        Collider2D[] firstExplosionRadius = Physics2D.OverlapCircleAll(transform.position, 2.5f, 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("VIPDamage") | 1 << LayerMask.NameToLayer("Player1") | 1 << LayerMask.NameToLayer("Boss"));
        Collider2D[] secondExplosionRadius = Physics2D.OverlapCircleAll(transform.position, 4f, 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("VIPDamage") | 1 << LayerMask.NameToLayer("Player1") | 1 << LayerMask.NameToLayer("Boss"));
        Collider2D[] tntExplosionRadius = Physics2D.OverlapCircleAll(transform.position, 3f, 1 << LayerMask.NameToLayer("Wall"));
        if (firstExplosionRadius != null)
        {
            for (int i = 0; i < firstExplosionRadius.Length; i++)
            {
                if (firstExplosionRadius[i] != null)
                {
                    if (!Physics2D.Raycast(transform.position, firstExplosionRadius[i].transform.position - transform.position, Vector3.Distance(transform.position, firstExplosionRadius[i].transform.position), 1 << LayerMask.NameToLayer("Wall")))
                    {
                        if (firstExplosionRadius[i].gameObject.GetComponent<MovingEnemy>() != null) {
                            firstExplosionRadius[i].gameObject.GetComponent<MovingEnemy>().DamageEnemy(grenadeDamage);
                        } else if (firstExplosionRadius[i].gameObject.GetComponent<Scientist>() != null) {
                            firstExplosionRadius[i].gameObject.GetComponent<Scientist>().DamageEnemy(grenadeDamage);
                        } else if (firstExplosionRadius[i].gameObject.transform.parent != null) {
                            if (firstExplosionRadius[i].gameObject.transform.parent.GetComponent<VIP>() != null) {
                                firstExplosionRadius[i].gameObject.transform.parent.GetComponent<VIP>().TakeDamage(grenadeDamage);
                            }
                        } else if (firstExplosionRadius[i].gameObject.GetComponent<PlayerMovement>() != null) {
                            firstExplosionRadius[i].gameObject.GetComponent<PlayerMovement>().TakeDamage(grenadeDamage);
                        } else if (firstExplosionRadius[i].gameObject.GetComponent<Boss>() != null) {
                            firstExplosionRadius[i].gameObject.GetComponent<Boss>().DamageEnemy(grenadeDamage * 3f);
                        }
                    }
                }
            }
        }

        if (tntExplosionRadius != null) {
            for (int i = 0; i < tntExplosionRadius.Length; i++) {
                if (tntExplosionRadius[i] != null) {
                    if (tntExplosionRadius[i].gameObject.tag == "TNT") {
                        tntExplosionRadius[i].gameObject.GetComponent<TogglableObject>().toggled = true;
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
                        if (secondExplosionRadius[i].gameObject.GetComponent<MovingEnemy>() != null) {
                            secondExplosionRadius[i].gameObject.GetComponent<MovingEnemy>().DamageEnemy(grenadeDamage);
                        } else if (secondExplosionRadius[i].gameObject.GetComponent<Scientist>() != null) {
                            secondExplosionRadius[i].gameObject.GetComponent<Scientist>().DamageEnemy(grenadeDamage);
                        } else if (secondExplosionRadius[i].gameObject.transform.parent != null) {
                            if (secondExplosionRadius[i].gameObject.transform.parent.GetComponent<VIP>() != null) {
                                secondExplosionRadius[i].gameObject.transform.parent.GetComponent<VIP>().TakeDamage(grenadeDamage);
                            }
                        } else if (secondExplosionRadius[i].gameObject.GetComponent<PlayerMovement>() != null) {
                            secondExplosionRadius[i].gameObject.GetComponent<PlayerMovement>().TakeDamage(grenadeDamage);
                        } else if (secondExplosionRadius[i].gameObject.GetComponent<Boss>() != null) {
                            secondExplosionRadius[i].gameObject.GetComponent<Boss>().DamageEnemy(grenadeDamage * 3f);
                        }
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

                    if (!Physics2D.Raycast(transform.position, explosionSoundWave[i].transform.position - transform.position, Vector3.Distance(transform.position, explosionSoundWave[i].transform.position), 1 << LayerMask.NameToLayer("Wall"))) {
                        if (explosionSoundWave[i].GetComponent<MovingEnemy>() != null) {
                            if (explosionSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.Moving ||
                        explosionSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.WaitingToMove ||
                        explosionSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.SearchingPlayer &&
                        explosionSoundWave[i].GetComponent<MovingEnemy>().GetRunningTime() >= 3f) {

                                explosionSoundWave[i].GetComponent<MovingEnemy>().playerSearchPosition = transform.position;
                                explosionSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing = MovingEnemy.CurrentStance.SearchingPlayer;
                            }
                        }
                    } 
                }
            }
        }

        Instantiate<GameObject>(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, 0f);
    }
}
