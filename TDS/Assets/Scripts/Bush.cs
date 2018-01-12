using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour {

    private List<GameObject> bushList;
    public GameObject leavesPrefab;

	void Start () {
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

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1")) {
            Instantiate<GameObject>(leavesPrefab, transform.position, Quaternion.identity);
            GameObject.Find("GameManager").GetComponent<SoundManager>().PlaySound("Leaf", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            bushList.Remove(collision.gameObject);
        }
    }
}
