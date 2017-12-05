using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour {

    private bool doneOnce = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!doneOnce) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player1")) {
                GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().NextObjective();
                doneOnce = true;
            }
        }
    }
}
