using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglableObject : MonoBehaviour {

    public bool toggled = true;
    public enum ObjectType { Laser, Gas, Door, Ammo, TNT};
    public ObjectType objectType;
    public Sprite offImage;
    public int grenadesGivenOnUse = 2;
    public List<GameObject> tntList;
    public float tntExplodeTime = 1f;
    public GameObject explosionPrefab;

    private List<GameObject> damageGasList;
    private bool playerInGas = false;
    private GameObject playerObject;
    private float runningGasTime = 0f;
    public float gasInterval = 0.2f;
    public int gasDamage = 2;
    private bool toggleDoorAnimation = false;
    private Animator animator;
    private bool doneOnce = false;
    private GameObject tntObject;
    private float runningTNTTime = 0.0f;




    private void Awake() {
        if (objectType == ObjectType.Door || objectType == ObjectType.Laser || objectType == ObjectType.Ammo) {
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
                                } else if (damageGasList[i].transform.parent.GetComponent<VIP>() != null) {
                                    damageGasList[i].transform.parent.GetComponent<VIP>().TakeDamage(gasDamage);
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
        }
    }

    private void ExplodeTNT() {
        ExplodeAllTNTInTheArea(4.0f);
        SpawnTNTExplosions();
    }

    private void SpawnTNTExplosions() {
        //spawn explosion particlesystems
        Instantiate<GameObject>(explosionPrefab, transform.position, Quaternion.identity);
        Instantiate<GameObject>(explosionPrefab, transform.position + new Vector3(0f, -1f, 0f), Quaternion.identity);
        Instantiate<GameObject>(explosionPrefab, transform.position + new Vector3(1f, 1f, 0f), Quaternion.identity);
        Instantiate<GameObject>(explosionPrefab, transform.position + new Vector3(-1f, 1f, 0f), Quaternion.identity);

        KillPlayerIfTooClose();

        for (int i = 0; i < tntList.Count; i++) {
            if (tntList[i] != null) {
                tntList[i].GetComponent<TogglableObject>().toggled = true;
            }
        }
        Destroy(gameObject);
    }

    private void ExplodeAllTNTInTheArea(float radius) {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 4f, 1 << LayerMask.NameToLayer("Wall"));

        if (col != null) {
            for (int i = 0; i < col.Length; i++) {
                if (col[i] != null && col[i].gameObject != tntObject) {
                    if (col[i].gameObject.tag == "TNT") {
                        tntList.Add(col[i].gameObject);
                    }
                }
            }
        }
    }

    private void KillPlayerIfTooClose() {
        if (Vector3.Distance(gameObject.transform.position, playerObject.transform.position) <= 3.7f) {
            playerObject.GetComponent<PlayerMovement>().TakeDamage(500f);
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
    }
}
