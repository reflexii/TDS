using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglableObject : MonoBehaviour {

    public bool toggled = true;
    public enum ObjectType { Laser, Gas, Door, Ammo};
    public ObjectType objectType;
    public Sprite offImage;

    private List<GameObject> damageGasList;
    private bool playerInGas = false;
    private GameObject playerObject;
    private float runningGasTime = 0f;
    public float gasInterval = 0.2f;
    public int gasDamage = 2;
    private bool toggleDoorAnimation = false;
    private Animator animator;
    private bool doneOnce = false;




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
                                damageGasList[i].GetComponent<MovingEnemy>().DamageEnemy(gasDamage);
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
                        Debug.Log("Fill Ammo");
                        doneOnce = true;
                        playerObject.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().FillMagazine();
                    } else {
                        toggled = false;
                    }
                    
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && objectType == ObjectType.Gas) {
            damageGasList.Add(collision.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1")) {
            playerInGas = true;
            playerObject = collision.gameObject;
            Debug.Log("asd");
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && objectType == ObjectType.Gas) {
            damageGasList.Remove(collision.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1")) {
            playerInGas = false;
        }
    }
}
