using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglableObject : MonoBehaviour {

    public bool toggled = true;
    public enum ObjectType { Laser, Gas, Door};
    public ObjectType objectType;
    public Sprite offImage;

    private List<GameObject> damageGasList;
    private float runningGasTime = 0f;
    public float gasInterval = 0.2f;
    public int gasDamage = 2;
    private bool toggleDoorAnimation = false;
    private Animator animator;




    private void Awake() {
        if (objectType == ObjectType.Door || objectType == ObjectType.Laser) {
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
                        transform.GetChild(i).gameObject.GetComponent<ParticleSystem>().Play();
                    }

                    if (runningGasTime >= gasInterval && damageGasList.Count >= 1) {
                        for (int i = 0; i < damageGasList.Count; i++) {
                            damageGasList[i].GetComponent<MovingEnemy>().DamageEnemy(gasDamage);
                        }
                        runningGasTime = 0f;
                    }
                } else
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                    for (int i = 0; i < transform.childCount; i++) {
                        transform.GetChild(i).gameObject.GetComponent<ParticleSystem>().Stop();
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && objectType == ObjectType.Gas) {
            damageGasList.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && objectType == ObjectType.Gas) {
            damageGasList.Remove(collision.gameObject);
        }
    }
}
