using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    public float toggleDelay = 1f;
    public bool togglable = false;
    public List<GameObject> parentObjects;
    public bool startToggled = false;
    public bool animated = true;

    private List<GameObject> allTogglableObjects;
    private float runningDelayTime = 2f;
    

    //animation
    private Animator animator;
    private bool toggleButtonAnimation;
    private bool pressedButton = false;

    private void Awake()
    {
        if (animated) {
            animator = GetComponent<Animator>();
        }
        
        allTogglableObjects = new List<GameObject>();
        AddTogglableObjectsToList();
        toggleButtonAnimation = !startToggled;
    }

    private void Start() {
        if (animated) {
            animator.SetBool("StartToggle", startToggled);
        }
        
    }

    void AddTogglableObjectsToList()
    {
        for (int i = 0; i < parentObjects.Count; i++)
        {
            //children objects
            foreach (Transform child in parentObjects[i].transform)
            {
                if (child.GetComponent<TogglableObject>() != null)
                {
                    allTogglableObjects.Add(child.gameObject);
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
        
    }

    public void Toggle()
    {
        if (runningDelayTime >= toggleDelay)
        {
            for (int i = 0; i < allTogglableObjects.Count; i++)
            {
                if (allTogglableObjects != null)
                {
                    if (allTogglableObjects[i] != null)
                    {
                        allTogglableObjects[i].GetComponent<TogglableObject>().toggled = !allTogglableObjects[i].GetComponent<TogglableObject>().toggled;
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
            
            runningDelayTime = 0f;
        }
    }

}
