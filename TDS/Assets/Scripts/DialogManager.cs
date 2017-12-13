using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DialogManager : MonoBehaviour {

    public List<string> dialogueList;
    public bool dialogueIsActive = false;
    public GameObject dialogueText;
    public GameObject dialogueImage;
    public TextAsset textAsset;
    public Animator animator;
    private int linesRead = 0;

    public GameObject currentDialogueTrigger;
    public GameObject dialogueParent;

    private void Start() {
        dialogueList = new List<string>();
        dialogueParent = GameObject.Find("Dialogue");
        dialogueText = GameObject.Find("DialogueText");
        dialogueImage = GameObject.Find("Speaker");
        animator = dialogueImage.GetComponent<Animator>();
        dialogueParent.SetActive(false);

        string[] linesInFile = textAsset.text.Split("\n"[0]);

        foreach (string line2 in linesInFile) {
            dialogueList.Add(line2);
            linesRead++;
        }
    }

    public string GetStringFromList(string keyString) {
        for (int i = 0; i < dialogueList.Count; i++) {
            if (dialogueList[i].Contains(keyString)) {
                string finaltext = dialogueList[i].Substring(keyString.Length + 1);
                return finaltext;
            }
        }
        return "ERROR: CANT FIND THE STRING.";
    }

    public void ToggleDialogueUIOn() {
        dialogueParent.SetActive(true);
        dialogueIsActive = true;
    }

    public void ToggleDialogueUIOff() {
        dialogueIsActive = false;
        dialogueParent.SetActive(false);

        if (currentDialogueTrigger != null) {
            if (currentDialogueTrigger.GetComponent<DialogueToggle>().triggerObjective) {
                GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().NextObjective();
            }
        }
    }

    public bool DialogueActive() {
        if (dialogueIsActive) {
            return true;
        } else {
            return false;
        }
    }
}
