using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertButton : MonoBehaviour {

    private Alert alert;

	void Awake () {
        alert = GameObject.Find("Alert").GetComponent<Alert>();
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && collision.transform.tag == "Scientist") {

            if (collision.gameObject.GetComponent<Scientist>().whatEnemyIsDoing == Scientist.CurrentStance.Alerted) {
                alert.AlertOn = true;
                collision.gameObject.GetComponent<Scientist>().whatEnemyIsDoing = Scientist.CurrentStance.Hiding;
            }
            
        }
    }
}
