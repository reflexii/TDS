using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour {

    public void DestroyWall()
    {
        Destroy(gameObject, 0f);
        //play animation
        //play sound
    }
}
