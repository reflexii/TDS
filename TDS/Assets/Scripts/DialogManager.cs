using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DialogManager : MonoBehaviour {

    public FileInfo sourceFile;
    public StreamReader reader;
    public string text;
    public List<string> dialogueList;

    private void Awake() {
        dialogueList = new List<string>();
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
}
