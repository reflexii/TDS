using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DialogManager : MonoBehaviour {

    public FileInfo sourceFile;
    public StreamReader reader;
    public List<string> dialogueList;
    public bool dialogueIsActive = false;

    private GameObject dialogueParent;
    private string text = "";

    private void Awake() {
        dialogueList = new List<string>();
        dialogueParent = GameObject.Find("Dialogue");
        dialogueParent.SetActive(false);
        sourceFile = new FileInfo("Assets/Text/Dialogue.txt");
        reader = sourceFile.OpenText();

        while (text != null) {
            text = reader.ReadLine();

            if (text != null) {
                dialogueList.Add(text);
            }
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
    }

    public bool DialogueActive() {
        if (dialogueIsActive) {
            return true;
        } else {
            return false;
        }
    }
}
