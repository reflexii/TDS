using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglableObject : MonoBehaviour {

    public bool toggled = true;
    public enum ObjectType { Laser, Gas, Door, Ammo, TNT, FlyingBox};
    public ObjectType objectType;
    public Sprite offImage;
    public int grenadesGivenOnUse = 2;
    public List<GameObject> tntList;
    public float tntExplodeTime = 1f;
    public GameObject explosionPrefab;
    public float tntDamage = 1000f;
    public GameObject objectThatToggledThis;
    public GameObject tntPrefab;

    private List<GameObject> damageGasList;
    private bool playerInGas = false;
    private GameObject playerObject;
    private float runningGasTime = 0f;
    public float gasInterval = 0.2f;
    public int gasDamage = 2;
    private bool toggleDoorAnimation = false;
    private Animator animator;
    public bool doneOnce = false;
    private GameObject tntObject;
    private float runningTNTTime = 0.0f;
    private GameManager gm;
    private bool doneOnce2 = false;
    private float runningFlyingTime = 0.0f;
    




    private void Awake() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (objectType == ObjectType.Door || objectType == ObjectType.Laser || objectType == ObjectType.Ammo || objectType == ObjectType.FlyingBox) {
            animator = GetComponent<Animator>();
        }
        damageGasList = new List<GameObject>();

        if (objectType == ObjectType.Door) {
            animator.SetBool("StartToggle", toggled);

            if (toggled) {
                GetComponent<BoxCollider2D>().enabled = false;
            } else {
                GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        if (objectType == ObjectType.TNT) {
            tntObject = gameObject;
        }

        playerObject = GameObject.Find("Player");
    }

    private void Update()
    {
        switch(objectType)
        {
            case ObjectType.Laser:
                if (toggled)
                {
                    animator.enabled = true;
                    transform.gameObject.layer = LayerMask.NameToLayer("Wall");
                    GetComponent<BoxCollider2D>().enabled = true;
                    
                } else
                {
                    animator.enabled = false;
                    GetComponent<BoxCollider2D>().enabled = false;
                    GetComponent<SpriteRenderer>().sprite = offImage;
                    transform.gameObject.layer = LayerMask.NameToLayer("Default");
                }
                break;
            case ObjectType.Gas:
                if (toggled)
                {
                    runningGasTime += Time.deltaTime;
                    GetComponent<BoxCollider2D>().enabled = true;
                    for (int i = 0; i < transform.childCount; i++) {
                        if (transform.GetChild(i).gameObject.GetComponent<ParticleSystem>().isStopped) {
                            transform.GetChild(i).gameObject.GetComponent<ParticleSystem>().Play();
                        }
                        
                    }

                    //damage enemies
                    if (runningGasTime >= gasInterval) {
                        if (damageGasList.Count >= 1) {
                            for (int i = 0; i < damageGasList.Count; i++) {
                                if (damageGasList[i].GetComponent<MovingEnemy>() != null) {
                                    damageGasList[i].GetComponent<MovingEnemy>().DamageEnemy(gasDamage);
                                } else if (damageGasList[i].GetComponent<Scientist>() != null) {
                                    damageGasList[i].GetComponent<Scientist>().DamageEnemy(gasDamage);
                                } else if (damageGasList[i].transform.parent != null) {
                                    if (damageGasList[i].transform.parent.GetComponent<VIP>() != null) {
                                        damageGasList[i].transform.parent.GetComponent<VIP>().TakeDamage(gasDamage);
                                    } 
                                } else if (damageGasList[i].GetComponent<Boss>() != null) {
                                    damageGasList[i].GetComponent<Boss>().DamageEnemy(gasDamage * 10f);
                                    damageGasList[i].GetComponent<Boss>().buttonTarget = objectThatToggledThis;
                                    damageGasList[i].GetComponent<Boss>().whatEnemyIsDoing = Boss.CurrentStance.ShootingAtComputer;
                                    if (!doneOnce2) {
                                        damageGasList[i].GetComponent<Boss>().gun.GetComponent<Gun>().FillMagazine();
                                        doneOnce2 = true;
                                    }
                                    
                                }


                            }
                        }

                        if (playerInGas) {
                            playerObject.GetComponent<PlayerMovement>().TakeDamage(gasDamage);
                        }

                        runningGasTime = 0f;
                    }
                } else
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                    for (int i = 0; i < transform.childCount; i++) {
                        if (transform.GetChild(i).gameObject.GetComponent<ParticleSystem>().isPlaying) {
                            transform.GetChild(i).gameObject.GetComponent<ParticleSystem>().Stop();
                        }
                        
                    }
                }
                break;
            case ObjectType.Door:

                animator.SetBool("doorToggled", toggleDoorAnimation);

                if (toggled) {
                    toggleDoorAnimation = true;
                    GetComponent<BoxCollider2D>().enabled = false;

                } else {
                    toggleDoorAnimation = false;
                    if (GetComponent<BoxCollider2D>().size.y >= 2f) {
                        GetComponent<BoxCollider2D>().enabled = true;
                    }  
                }
                break;
            case ObjectType.Ammo:
                animator.SetBool("Trigger", toggled);

                if (toggled && !doneOnce) {
                    if (playerObject.GetComponent<PlayerMovement>().hasGun) {
                        Debug.Log("Fill Ammo and grenades");
                        doneOnce = true;
                        playerObject.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().FillMagazine();
                        playerObject.GetComponent<PlayerMovement>().currentGrenadeAmount += grenadesGivenOnUse;
                        playerObject.GetComponent<PlayerMovement>().grenadeText.text = "" + playerObject.GetComponent<PlayerMovement>().currentGrenadeAmount;
                        gm.GetComponent<SoundManager>().PlaySound("AmmoCrate", false);
                    } else {
                        toggled = false;
                    }
                    
                }
                break;
            case ObjectType.TNT:

                if (toggled) {
                    runningTNTTime += Time.deltaTime;
                }

                if (toggled && !doneOnce && runningTNTTime >= tntExplodeTime) {
                    doneOnce = true;
                    ExplodeTNT();
                }
                break;
            case ObjectType.FlyingBox:
                if (toggled) {
                    animator.SetBool("Toggled", true);

                    //spawn box after time x
                    runningFlyingTime += Time.deltaTime;

                    if (runningFlyingTime >= 2.3f && !doneOnce) {
                        Instantiate<GameObject>(tntPrefab, transform.position, Quaternion.identity);
                        doneOnce = true;
                    }
                }
                break;
        }
    }

    private void ExplodeTNT() {
        ExplodeAllTNTInTheArea(4f);
        SpawnTNTExplosions();
    }

    private void SpawnTNTExplosions() {
        //spawn explosion particlesystems
        Instantiate<GameObject>(explosionPrefab, transform.position, Quaternion.identity);
        Instantiate<GameObject>(explosionPrefab, transform.position + new Vector3(0f, -1f, 0f), Quaternion.identity);
        Instantiate<GameObject>(explosionPrefab, transform.position + new Vector3(1f, 1f, 0f), Quaternion.identity);
        Instantiate<GameObject>(explosionPrefab, transform.position + new Vector3(-1f, 1f, 0f), Quaternion.identity);

        gm.GetComponent<SoundManager>().PlaySound("TNT", true);

        KillPlayerIfTooClose();
        KillEnemiesAndVipIfTooClose();

        for (int i = 0; i < tntList.Count; i++) {
            if (tntList[i] != null) {
                tntList[i].GetComponent<TogglableObject>().toggled = true;
            }
        }
        Destroy(gameObject);
    }

    private void ExplodeAllTNTInTheArea(float radius) {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerMask.NameToLayer("Wall"));

        if (col != null) {
            for (int i = 0; i < col.Length; i++) {
                if (col[i] != null && col[i].gameObject != tntObject) {
                    if (col[i].gameObject.tag == "TNT") {
                        tntList.Add(col[i].gameObject);
                    } else if (col[i].gameObject.tag == "Destructable" || col[i].gameObject.tag == "SeeThroughDestructable") {
                        col[i].gameObject.GetComponent<DestroyableObject>().TakeDamage(tntDamage);
                    }
                }
            }
        }
    }

    private void KillPlayerIfTooClose() {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, playerObject.transform.position - transform.position, Vector3.Distance(transform.position, playerObject.transform.position), 1 << LayerMask.NameToLayer("Wall"));
        bool hitPlayer = true;

        if (hit != null) {
            for (int i = 0; i < hit.Length; i++) {
                if (hit[i].transform.gameObject != tntObject) {
                    hitPlayer = false;
                }
            }
        }

        if (hitPlayer && Vector3.Distance(gameObject.transform.position, playerObject.transform.position) <= 3.7f) {
            playerObject.GetComponent<PlayerMovement>().TakeDamage(tntDamage);
        }
    }

    private void KillEnemiesAndVipIfTooClose() {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 3f, 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("VIPDamage") | 1 << LayerMask.NameToLayer("Boss"));

        if (col != null) {
            for (int i = 0; i < col.Length; i++) {
                if (col[i] != null) {

                    RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, col[i].transform.position - transform.position, Vector3.Distance(transform.position, col[i].transform.position), 1 << LayerMask.NameToLayer("Wall"));
                    bool hitTarget = true;

                    if (hit != null) {
                        for (int j = 0; j < hit.Length; j++) {
                            if (hit[j].transform.gameObject != tntObject) {
                                hitTarget = false;
                            }
                        }
                    }

                    if (col[i].gameObject.tag == "Enemy" && hitTarget) {
                        col[i].GetComponent<MovingEnemy>().DamageEnemy(tntDamage);
                    } else if (col[i].gameObject.tag == "Scientist" && hitTarget) {
                        col[i].GetComponent<Scientist>().DamageEnemy(tntDamage);
                    } else if (col[i].gameObject.transform.parent != null) {
                        if (col[i].gameObject.transform.parent.gameObject.layer == LayerMask.NameToLayer("VIP") && hitTarget) {
                            col[i].transform.parent.GetComponent<VIP>().TakeDamage(tntDamage);
                        }
                    } else if (col[i].gameObject.layer == LayerMask.NameToLayer("Boss")) {
                        col[i].GetComponent<Boss>().DamageEnemy(tntDamage * 1.5f);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && objectType == ObjectType.Gas) {
            damageGasList.Add(collision.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1") && objectType == ObjectType.Gas) {
            playerInGas = true;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("VIPDamage") && objectType == ObjectType.Gas) {
            damageGasList.Add(collision.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Boss") && objectType == ObjectType.Gas) {
            damageGasList.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && objectType == ObjectType.Gas) {
            damageGasList.Remove(collision.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1") && objectType == ObjectType.Gas) {
            playerInGas = false;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("VIPDamage") && objectType == ObjectType.Gas) {
            damageGasList.Remove(collision.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Boss") && objectType == ObjectType.Gas) {
            damageGasList.Remove(collision.gameObject);
            doneOnce2 = false;
        }
    }
}
