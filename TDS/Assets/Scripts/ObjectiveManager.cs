using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour {

    public List<string> questList;
    public string questSearchedString;
    public TextAsset textAsset;
    public bool objectivesComplete = false;
    public bool missionFailed = false;
    public bool killAllMission = false;
    public int enemiesLeft;
    public bool timer = false;
    public float timerTime = 60f;
    public bool killTargetedEnemyMission = false;
    public bool pressTargetedButtonMission = false;

    private Text objectiveText;
    private Text objectiveTextBig;
    private int linesRead = 0;
    private int currentObjective = 1;
    private int objectiveCount;
    private Text objectiveNumberText;
    private GameObject deathScreenParent;
    private GameManager gm;
    private string failTextOnce = "";
    private bool doneOnce = false;
    private bool doneOnce2 = false;
    private bool doneOnce3 = false;

	void Start () {
        objectiveText = GameObject.Find("ObjectiveText").GetComponent<Text>();
        objectiveTextBig = GameObject.Find("Objective").GetComponent<Text>();
        objectiveNumberText = GameObject.Find("ObjectiveNumber").GetComponent<Text>();
        deathScreenParent = GameObject.Find("DeathScreen_parent");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        deathScreenParent.SetActive(false);
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

            FlashObjective();
        }
    }

    public void KillAllMission() {
        if (killAllMission) {
            if (!doneOnce) {
                FindAllEnemies();
                doneOnce = true;
            }

            if (enemiesLeft <= 0 && !doneOnce2) {
                NextObjective();
                doneOnce2 = true;
            }
        }
    }

    public void Timer() {
        if (timer) {
            timerTime -= Time.deltaTime;

            //add ui.text modification

        if (timerTime <= 0f && !doneOnce3) {
                MissionFailed("You ran out of time.");
                doneOnce3 = true;
            }
        }
    }

    private void FindAllEnemies() {
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

        foreach(GameObject g in gos) {
            if (g.layer == LayerMask.NameToLayer("Enemy")) {
                if (g.GetComponent<MovingEnemy>() != null) {
                    enemiesLeft++;
                }
            }
        }
    }

    public void FlashObjective() {
        AnimatorStateInfo info = objectiveTextBig.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        int stateHash = info.fullPathHash;
        objectiveTextBig.gameObject.GetComponent<Animator>().Play(stateHash, 0, 0.0f);
        Debug.Log("flash");
    }

    private void Update() {
        UpdateObjectiveNumberText();
        KillAllMission();
        Timer();
    }

    public void MissionFailed(string failText) {
        missionFailed = true;
        deathScreenParent.SetActive(true);

        if (failTextOnce.Equals("")) {
            deathScreenParent.transform.Find("FailExplanation").GetComponent<Text>().text = failText;
            failTextOnce = failText;
        }
        gm.missionFailed = true;
    }

    void UpdateObjectiveNumberText() {
        if (objectiveNumberText != null) {
            objectiveNumberText.text = (currentObjective-1) + "/" + (objectiveCount-1);
        }
        
    }

}
