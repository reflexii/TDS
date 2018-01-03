using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueToggle : MonoBehaviour {

    public enum Talker { Moustache, VIP, Player, Boss, Scientist };
    public Talker talker;
    private int talkerInt;
    private GameManager gameManager;
    private bool didOnce = false;
    private Text dialogueText;
    private Animator animator;

    public string searchedCode = "";
    public bool useOneByOne = false;
    public bool useToTrigger = false;
    public float waitTimeInBetweenLetters = 0.02f;
    public bool isDone = true;
    [HideInInspector]
    public bool showE = false;
    public bool triggerObjective = false;
    public bool stopText = false;
    public bool vipTrigger = false;

    private void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dialogueText = gameManager.gameObject.GetComponent<DialogManager>().dialogueText.GetComponent<Text>();
        animator = gameManager.gameObject.GetComponent<DialogManager>().animator;

        if (useToTrigger) {
            showE = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!didOnce && collision.gameObject.layer == LayerMask.NameToLayer("Player1") && !useToTrigger && !vipTrigger) {
            gameManager.gameObject.GetComponent<DialogManager>().ToggleDialogueUIOn();

            if (gameManager.GetComponent<DialogManager>().dialogueOnTheWay && gameManager.GetComponent<DialogManager>().currentDialogueTrigger != null) {
                gameManager.GetComponent<DialogManager>().currentDialogueTrigger.GetComponent<DialogueToggle>().StopCoroutine("OneCharacterAtATime");
            }

            gameManager.GetComponent<DialogManager>().currentDialogueTrigger = gameObject;
            switch (talker) {
                case Talker.Moustache:
                    talkerInt = 1;
                    break;
                case Talker.Player:
                    talkerInt = 2;
                    break;
                case Talker.VIP:
                    talkerInt = 3;
                    break;
                case Talker.Boss:
                    talkerInt = 4;
                    break;
                case Talker.Scientist:
                    talkerInt = 5;
                    break;
            }

            animator.SetInteger("TalkerInt", talkerInt);

            if (!useOneByOne) {
                dialogueText.text = gameManager.gameObject.GetComponent<DialogManager>().GetStringFromList(searchedCode);
            } else {
                StartCoroutine("OneCharacterAtATime", searchedCode);
            }
            
            didOnce = true;
        }
        
        if (vipTrigger && !didOnce && collision.gameObject.layer == LayerMask.NameToLayer("VIP")) {
            gameManager.gameObject.GetComponent<DialogManager>().ToggleDialogueUIOn();

            if (gameManager.GetComponent<DialogManager>().dialogueOnTheWay && gameManager.GetComponent<DialogManager>().currentDialogueTrigger != null) {
                gameManager.GetComponent<DialogManager>().currentDialogueTrigger.GetComponent<DialogueToggle>().StopCoroutine("OneCharacterAtATime");
            }

            gameManager.GetComponent<DialogManager>().currentDialogueTrigger = gameObject;
            switch (talker) {
                case Talker.Moustache:
                    talkerInt = 1;
                    break;
                case Talker.Player:
                    talkerInt = 2;
                    break;
                case Talker.VIP:
                    talkerInt = 3;
                    break;
                case Talker.Boss:
                    talkerInt = 4;
                    break;
                case Talker.Scientist:
                    talkerInt = 5;
                    break;
            }

            animator.SetInteger("TalkerInt", talkerInt);

            if (!useOneByOne) {
                dialogueText.text = gameManager.gameObject.GetComponent<DialogManager>().GetStringFromList(searchedCode);
            } else {
                StartCoroutine("OneCharacterAtATime", searchedCode);
            }

            didOnce = true;
        }
    }

    public void TriggerDialogue() {
        if (useToTrigger) {
            gameManager.gameObject.GetComponent<DialogManager>().ToggleDialogueUIOn();

            if (gameManager.GetComponent<DialogManager>().dialogueOnTheWay && gameManager.GetComponent<DialogManager>().currentDialogueTrigger != null) {
                gameManager.GetComponent<DialogManager>().currentDialogueTrigger.GetComponent<DialogueToggle>().StopCoroutine("OneCharacterAtATime");
            }

            gameManager.GetComponent<DialogManager>().currentDialogueTrigger = gameObject;
            switch (talker) {
                case Talker.Moustache:
                    talkerInt = 1;
                    break;
                case Talker.Player:
                    talkerInt = 2;
                    break;
                case Talker.VIP:
                    talkerInt = 3;
                    break;
                case Talker.Boss:
                    talkerInt = 4;
                    break;
                case Talker.Scientist:
                    talkerInt = 5;
                    break;
            }

            animator.SetInteger("TalkerInt", talkerInt);

            gameManager.GetComponent<DialogManager>().currentDialogueTrigger = gameObject;

            if (!useOneByOne) {
                dialogueText.text = gameManager.gameObject.GetComponent<DialogManager>().GetStringFromList(searchedCode);
            } else {
                StartCoroutine("OneCharacterAtATime", searchedCode);
            }
            
        }
    }

    IEnumerator OneCharacterAtATime(string keyCode) {
        string text = gameManager.gameObject.GetComponent<DialogManager>().GetStringFromList(keyCode);
        char[] textOneByOne = text.ToCharArray();
        dialogueText.text = "";

        for (int i = 0; i < textOneByOne.Length; i++) {
            dialogueText.text += textOneByOne[i];
            isDone = false;
            gameManager.gameObject.GetComponent<DialogManager>().dialogueOnTheWay = true;

            yield return new WaitForSeconds(waitTimeInBetweenLetters);
        }
        gameManager.gameObject.GetComponent<DialogManager>().dialogueOnTheWay = false;
        isDone = true;

    }
}
