using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

    public float grenadeSpeed;
    public Vector3 grenadeMoveDirection = Vector3.up;
    public float grenadeVelocityReductionSpeed = 5f;
    public float timeBeforeGrenadeExplosion = 4f;

    private float runningGrenadeTime = 0f;
	
	void Update () {
        runningGrenadeTime += Time.deltaTime;
        grenadeSpeed -= Time.deltaTime * grenadeVelocityReductionSpeed;

        if (grenadeSpeed <= 0f)
        {
            grenadeSpeed = 0f;
        }

        gameObject.transform.position += grenadeMoveDirection.normalized * Time.deltaTime * grenadeSpeed;

        if (runningGrenadeTime >= timeBeforeGrenadeExplosion)
        {
            ExplodeGrenade(); 
        }
        
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            ExplodeGrenade();
        }
    }

    public void ExplodeGrenade()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (colliders != null)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null && colliders[i].gameObject.tag == "Destructable")
                {
                    colliders[i].gameObject.GetComponent<DestroyableObject>().DestroyWall();
                }
            }
        }

        //play sound
        //play animation
        Destroy(gameObject, 0f);
    }
}
