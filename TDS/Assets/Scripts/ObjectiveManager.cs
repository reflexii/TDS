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
    public bool toggleObjectsAfterTimer = false;
    public List<GameObject> togglableObjects;
    public bool killTargetedEnemyMission = false;
    public bool pressTargetedButtonMission = false;

    private Text objectiveText;
    private Text objectiveTextBig;
    private int linesRead = 0;
    private int currentObjective = 1;
    private int objectiveCount;
    private Text objectiveNumberText;
    public GameObject deathScreenParent;
    private GameManager gm;
    private string failTextOnce = "";
    private bool doneOnce = false;
    private bool doneOnce2 = false;
    private bool doneOnce3 = false;
    private Text timerText;
    private Image timerBG;
    private Image timerImage;
    private bool timerFlashing = false;

	void Start () {
        objectiveText = GameObject.Find("ObjectiveText").GetComponent<Text>();
        objectiveTextBig = GameObject.Find("Objective").GetComponent<Text>();
        objectiveNumberText = GameObject.Find("ObjectiveNumber").GetComponent<Text>();
        deathScreenParent = GameObject.Find("DeathScreen_parent");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        timerText = GameObject.Find("TimerText").GetComponent<Text>();
        timerBG = GameObject.Find("TimerBG").GetComponent<Image>();
        timerImage = GameObject.Find("TimerImage").GetComponent<Image>();
        timerText.enabled = false;
        questList = new List<string>();

        for (int i = 0; i < deathScreenParent.transform.childCount; i++) {
            if (deathScreenParent.transform.GetChild(i).GetComponent<Image>() != null) {
                deathScreenParent.transform.GetChild(i).GetComponent<Image>().enabled = false;
            }
            if (deathScreenParent.transform.GetChild(i).GetComponent<Text>() != null) {
                deathScreenParent.transform.GetChild(i).GetComponent<Text>().enabled = false;
            }
        }

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

        if (!timer) {
            timerText.enabled = false;
            timerBG.enabled = false;
            timerImage.enabled = false;
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
            if (timerTime > 0.1f) {
                timerTime -= Time.deltaTime;
            } else {
                timerTime = 0f;
            }
            

            if (!missionFailed) {
                timerText.enabled = true;
                timerImage.enabled = true;
                timerBG.enabled = true;
            } else {
                timerText.enabled = false;
                timerImage.enabled = false;
                timerBG.enabled = false;
            }

            if (timerTime <= 10.1f && timerTime > 0.1f) {
                timerFlashing = true;
            } else if (timerTime <= 0.1f) {
                timerFlashing = false;
            }

            string temp = "" + timerTime;
            if (temp.Length >= 3 && timerTime < 10f) {
                temp = temp.Substring(0, 3);
            } else if (temp.Length >= 3 && timerTime >= 10f && timerTime < 100f) {
                temp = temp.Substring(0, 4);
            } else if (temp.Length >= 3 && timerTime >= 100f) {
                temp = temp.Substring(0, 5);
            }
            
            timerText.text = temp;

        if (timerTime <= 0f && !doneOnce3) {
                if (!toggleObjectsAfterTimer) {
                    MissionFailed("You ran out of time.");
                } else {
                    if (togglableObjects != null) {
                        for (int i = 0; i < togglableObjects.Count; i++) {
                            togglableObjects[i].GetComponent<Button2>().Toggle();
                        }
                    }
                }
                
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
    }

    private void Update() {
        UpdateObjectiveNumberText();
        KillAllMission();
        Timer();
        timerImage.gameObject.GetComponent<Animator>().SetBool("TimerFlashing", timerFlashing);
    }

    public void MissionFailed(string failText) {
        missionFailed = true;
        for (int i = 0; i < deathScreenParent.transform.childCount; i++) {
            if (deathScreenParent.transform.GetChild(i).GetComponent<Image>() != null) {
                deathScreenParent.transform.GetChild(i).GetComponent<Image>().enabled = true;
            }
            if (deathScreenParent.transform.GetChild(i).GetComponent<Text>() != null) {
                deathScreenParent.transform.GetChild(i).GetComponent<Text>().enabled = true;
            }
        }

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
