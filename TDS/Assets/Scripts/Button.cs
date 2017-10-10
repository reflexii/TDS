using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    public float toggleDelay = 1f;
    public bool togglable = false;
    public List<GameObject> parentObjects;

    private List<GameObject> allTogglableObjects;
    private float runningDelayTime = 2f;

    private void Awake()
    {
        allTogglableObjects = new List<GameObject>();
        AddTogglableObjectsToList();
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

            runningDelayTime = 0f;
        }
    }

}
