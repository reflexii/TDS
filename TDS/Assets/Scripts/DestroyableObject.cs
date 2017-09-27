using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {

	}

    public void DestroyWall()
    {
        Destroy(gameObject, 0f);
        //play animation
        //play sound
    }
}
