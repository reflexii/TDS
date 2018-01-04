using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour {

    private BoxCollider2D col;
    private List<GameObject> bushList;

	void Start () {
        col = GetComponent<BoxCollider2D>();
        bushList = new List<GameObject>();
	}
	
	void Update () {
		if (bushList.Count == 0) {
            gameObject.layer = LayerMask.NameToLayer("BushBlocker");
        } else {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            bushList.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            bushList.Remove(collision.gameObject);
        }
    }
}
