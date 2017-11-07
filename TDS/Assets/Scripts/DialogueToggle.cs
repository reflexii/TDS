using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueToggle : MonoBehaviour {


    private GameManager gameManager;
    private bool didOnce = false;
    private Text dialogueText;

    public string searchedCode = "";
    public bool useToTrigger = false;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dialogueText = GameObject.Find("DialogueText").GetComponent<Text>();
    }

    void Start () {
		
	}
	
	void Update () {

	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!didOnce && collision.gameObject.layer == LayerMask.NameToLayer("Player1") && !useToTrigger) {
            gameManager.gameObject.GetComponent<DialogManager>().ToggleDialogueUIOn();
            dialogueText.text = gameManager.gameObject.GetComponent<DialogManager>().GetStringFromList(searchedCode);
            didOnce = true;
        }   
    }

    public void TriggerDialogue() {
        if (useToTrigger) {
            gameManager.gameObject.GetComponent<DialogManager>().ToggleDialogueUIOn();
            dialogueText.text = gameManager.gameObject.GetComponent<DialogManager>().GetStringFromList(searchedCode);
        }
    }
}
