using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour {

    private bool doneOnce = false;
    public bool vipTrigger = false;
    public bool activateTimer = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!doneOnce) {

            if (!vipTrigger) {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Player1")) {
                    GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().NextObjective();

                    if (activateTimer) {
                        GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().timer = true;
                    }
                    doneOnce = true;
                }
            } else {
                if (collision.gameObject.layer == LayerMask.NameToLayer("VIP")) {
                    GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().NextObjective();

                    if (activateTimer) {
                        GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().timer = true;
                    }

                    doneOnce = true;
                }
            }
            
        }
    }
}
