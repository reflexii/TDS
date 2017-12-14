using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour {

    public GameObject fakeParent;
    public GameObject dialogueToggle;
    private bool doneOnce = false;
    public float timeBeforeDialogue;
    private float runningDialogueTime = 0.0f;
    private bool dialogue = false;

	void Start () {
		
	}
	
	void Update () {
		if (dialogue) {
            runningDialogueTime += Time.deltaTime;

            if (runningDialogueTime >= timeBeforeDialogue) {
                dialogueToggle.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1") && !doneOnce) {
            dialogue = true;
            collision.gameObject.GetComponent<PlayerMovement>().stopPlayer = true;
            collision.gameObject.GetComponent<PlayerMovement>().walking = false;
            for (int i = 0; i < fakeParent.transform.childCount; i++) {
                fakeParent.transform.GetChild(i).GetComponent<CutSceneEnemy>().walking = true;
            }
            doneOnce = true;
        }
    }
}
