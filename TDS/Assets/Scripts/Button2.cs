using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button2 : MonoBehaviour {

    public float toggleDelay = 1f;
    public bool togglable = false;
    public List<GameObject> parentObjects;
    public bool startToggled = false;
    public bool animated = true;
    public bool toggleObjectiveOnFirstUse = false;
    public bool toggleTimer = false;
    public bool alertOnToggle = false;
    public bool walkTrigger = false;
    [HideInInspector]
    public bool showE = true;
    public bool togglableOnlyOnce = false;
    public bool broken = false;
    public bool breakableWithEnemyBullets = false;

    private List<GameObject> allTogglableObjects;
    private float runningDelayTime = 2f;
    private Alert alert;
    
    //animation
    private Animator animator;
    private bool toggleButtonAnimation;
    private bool pressedButton = false;
    private bool doneOnce = false;
    private bool doneOnce2 = false;
    private ObjectiveManager om;

    private void Awake()
    {
        if (animated) {
            animator = GetComponent<Animator>();
        }
        if (alertOnToggle) {
            alert = GameObject.Find("Alert").GetComponent<Alert>();
        }

        if (walkTrigger) {
            showE = false;
        }
        
        allTogglableObjects = new List<GameObject>();
        AddTogglableObjectsToList();
        toggleButtonAnimation = !startToggled;
    }

    private void Start() {
        if (animated) {
            animator.SetBool("StartToggle", startToggled);
        }

        om = GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>();
    }

    void AddTogglableObjectsToList()
    {
        for (int i = 0; i < parentObjects.Count; i++)
        {
            //children objects
            if (parentObjects[i].transform.childCount > 0) {
                foreach (Transform child in parentObjects[i].transform) {
                    if (child.GetComponent<TogglableObject>() != null) {
                        allTogglableObjects.Add(child.gameObject);
                    }

                }
            }
            
            //parents
            if (parentObjects[i].GetComponent<TogglableObject>() != null)
            {
                allTogglableObjects.Add(parentObjects[i]);
            }
        }
        
    }

	void Update () {
        runningDelayTime += Time.deltaTime;

        if (animated) {
            animator.SetBool("Toggled", toggleButtonAnimation);
            animator.SetBool("PressedButton", pressedButton);
        }

        if (broken) {
            animator.SetBool("Broken", broken);
        }
        
    }

    public void Toggle()
    {
        if (runningDelayTime >= toggleDelay)
        {
            if (allTogglableObjects.Count >= 1) {
                for (int i = 0; i < allTogglableObjects.Count; i++) {
                    if (allTogglableObjects != null) {
                        if (allTogglableObjects[i] != null) {
                            allTogglableObjects[i].GetComponent<TogglableObject>().toggled = !allTogglableObjects[i].GetComponent<TogglableObject>().toggled;
                            allTogglableObjects[i].GetComponent<TogglableObject>().objectThatToggledThis = gameObject;
                        }
                    }
                }
            }

            if (animated) {
                if (!pressedButton) {
                    pressedButton = true;
                } else {
                    toggleButtonAnimation = !toggleButtonAnimation;
                }
            }

            if (togglableOnlyOnce) {
                GetComponent<BoxCollider2D>().enabled = false;
            }

            if (alertOnToggle) {
                alert.AlertOn = true;
            }

            if (toggleObjectiveOnFirstUse && !doneOnce) {
                om.NextObjective();

                if (toggleTimer) {
                    om.timer = !om.timer;
                }

                doneOnce = true;
            }

            runningDelayTime = 0f;
        }
    }

    public void ToggleOff() {
        if (runningDelayTime >= toggleDelay) {
            if (allTogglableObjects.Count >= 1) {
                for (int i = 0; i < allTogglableObjects.Count; i++) {
                    if (allTogglableObjects != null) {
                        if (allTogglableObjects[i] != null) {
                            allTogglableObjects[i].GetComponent<TogglableObject>().toggled = false;
                            allTogglableObjects[i].GetComponent<TogglableObject>().objectThatToggledThis = gameObject;
                        }
                    }
                }
            }

            runningDelayTime = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && collision.transform.tag == "Scientist") {
            if (collision.gameObject.GetComponent<Scientist>().whatEnemyIsDoing == Scientist.CurrentStance.Alerted) {
                Toggle();
                collision.gameObject.GetComponent<Scientist>().whatEnemyIsDoing = Scientist.CurrentStance.Hiding;
            }

        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1")) {
            if (walkTrigger && !doneOnce2) {
                Toggle();
                doneOnce2 = true;
            }
        }
    }

}
