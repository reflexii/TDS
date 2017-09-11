using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float bulletSpeed;
    public Vector3 direction;
	
	void Update () {
        transform.position += direction.normalized * bulletSpeed * Time.deltaTime;
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            Destroy(gameObject, 0f);
        }
    }
}
