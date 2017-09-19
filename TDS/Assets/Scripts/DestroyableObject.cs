using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour {

    float runningTime = 0f;
    public float destroyThisObjectTime = 4f;

	void Start () {
		
	}
	
	void Update () {
        runningTime += Time.deltaTime;

        if (runningTime > destroyThisObjectTime)
        {
            Destroy(this.gameObject, 0f);
        }
	}
}
