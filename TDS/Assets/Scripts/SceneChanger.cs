using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

    public string sceneName = "";
    public float timeBeforeSwitching;
    public bool mainMenu = false;

    private ObjectiveManager om;
    private GameManager gm;
    private bool scenesChanging = false;
    private float runningSwitchTime = 0.0f;
    private MainMenuScript mms;

    private void Start() {
        if (!mainMenu) {
            om = GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>();
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        } else {
            mms = GameObject.Find("MainMenuScript").GetComponent<MainMenuScript>();
        }
    }

    private void Update() {
        if (scenesChanging) {
            ChangeScene();
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (om.objectivesComplete && collision.gameObject.layer == LayerMask.NameToLayer("Player1")) {
            scenesChanging = true;
        }
    }

    void ChangeScene() {
        runningSwitchTime += Time.deltaTime;

        if (runningSwitchTime >= timeBeforeSwitching) {
            gm.NextLevelPreferences();
            StopAllSounds();

            SceneManager.LoadScene(sceneName);
        }
    }

    public void ChangeToFirstMap() {
        mms.ClearKeys();
        mms.mainMenu = false;
        SceneManager.LoadScene("1stmap");
    }

    public void ContinueLevel() {
        string name = "";
        mms.mainMenu = false;

        switch (mms.currentLevel) {
            case 1:
                name = "1stmap";
                break;
            case 2:
                name = "layerwarehouse";
                break;
            case 3:
                name = "backdoorgrenade";
                break;
            case 4:
                name = "gaslab";
                break;
            case 5:
                name = "glassfloor";
                break;
            case 6:
                name = "alertlab";
                break;
            case 7:
                name = "redalert";
                break;
            case 8:
                name = "1stmap";
                break;
            case 9:
                name = "1stmap";
                break;
            case 10:
                name = "1stmap";
                break;
            case 11:
                name = "1stmap";
                break;
            case 12:
                name = "1stmap";
                break;
            case 13:
                name = "1stmap";
                break;
            case 14:
                name = "1stmap";
                break;
            case 15:
                name = "1stmap";
                break;

        }

        SceneManager.LoadScene(name);

    }

    public void StopAllSounds() {
        GameObject audio = GameObject.Find("Audio");
        for (int i = 0; i < audio.transform.childCount; i++) {
            audio.transform.GetChild(i).GetComponent<AudioSource>().Stop();
        }
        Debug.Log("StopAll");
    }
}
