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
            col.enabled = true;
        } else {
            col.enabled = false;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            Debug.Log("Enter");
            bushList.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        bushList.Remove(collision.gameObject);
    }
}
