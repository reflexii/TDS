using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    public float toggleDelay = 2f;
    public bool togglable = false;
    public List<GameObject> toggledItems;

    private void Awake()
    {
        toggledItems = new List<GameObject>();
    }

    private float runningDelayTime = 2f;
	
	void Update () {
        runningDelayTime += Time.deltaTime;
	}

    public void Toggle()
    {
        if (runningDelayTime >= toggleDelay)
        {
            for (int i = 0; i < toggledItems.Count; i++)
            {
                if (toggledItems != null)
                {
                    if (toggledItems[i] != null)
                    {
                        toggledItems[i].GetComponent<TogglableObject>().toggled = !toggledItems[i].GetComponent<TogglableObject>().toggled;
                    }
                }
            }

            runningDelayTime = 0f;
        }
    }

}
