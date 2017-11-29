using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour {

    public List<string> questList;
    public string questSearchedString;
    public TextAsset textAsset;
    public bool objectivesComplete = false;

    private Text objectiveText;
    private int linesRead = 0;
    private int currentObjective = 1;
    private int objectiveCount;

	void Start () {
        objectiveText = GameObject.Find("ObjectiveText").GetComponent<Text>();
        questList = new List<string>();

        string[] linesInFile = textAsset.text.Split("\n"[0]);

        foreach (string line2 in linesInFile) {
            if (line2.Contains(questSearchedString)) {
                questList.Add(line2.Substring(questSearchedString.Length + 1));
                objectiveCount++;
            }
            linesRead++;
        }

        if (questList[0] != null) {
            objectiveText.text = questList[0];
        } else {
            objectiveText.text = "Quests not found.";
        }
	}

    public void NextObjective() {
        if (currentObjective < objectiveCount) {
            currentObjective++;
            objectiveText.text = questList[currentObjective - 1];

            if (currentObjective == objectiveCount) {
                objectivesComplete = true;
            }
        }
    }

}
