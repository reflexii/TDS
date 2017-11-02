using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour {

    public GameObject ps;
    public bool breakableWithBullets = false;

    public void DestroyWall()
    {
        Instantiate<GameObject>(ps, transform.position, Quaternion.identity);
        Destroy(gameObject, 0f);
        //play animation
        //play sound
    }
}
