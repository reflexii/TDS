using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour {

    public string sceneName = "";
    public float timeBeforeSwitching;
    public bool mainMenu = false;

    private ObjectiveManager om;
    private GameManager gm;
    private bool scenesChanging = false;
    private float runningSwitchTime = 0.0f;
    private MainMenuScript mms;
    private bool fadedOnce = false;
    private GameObject canvasObject;

    private void Start() {
        if (!mainMenu) {
            om = GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>();
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        } else {
            mms = GameObject.Find("MainMenuScript").GetComponent<MainMenuScript>();

            if (GameObject.Find("Fade") != null) {
                GameObject.Find("Fade").GetComponent<Fade>().fadeValue = 1f;
                GameObject.Find("Fade").GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
            }
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

        GameObject.Find("MissionComplete").transform.GetChild(0).GetComponent<Text>().color = new Color(0f, 0f, 0f, 1f);
        GameObject.Find("MissionComplete").transform.GetChild(1).GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);

        if (!fadedOnce && runningSwitchTime >= 0.5f) {
            GameObject.Find("Fade").GetComponent<Fade>().StartFadeOut();
            fadedOnce = true;
        }
        
        //Mission complete

        runningSwitchTime += Time.deltaTime;

        if (runningSwitchTime >= timeBeforeSwitching) {
            gm.NextLevelPreferences();
            StopAllSounds();

            GameObject.Find("Fade").GetComponent<Fade>().StartFadeIn();
            GameObject.Find("MissionComplete").transform.GetChild(0).GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
            GameObject.Find("MissionComplete").transform.GetChild(1).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
            SceneManager.LoadScene(sceneName);
        }
    }

    public void ChangeToFirstMap() {
        mms.ClearKeys();
        mms.mainMenu = false;
        if (canvasObject != null) {
            canvasObject.SetActive(true);
        }
        if (GameObject.Find("Pause") != null) {
            GameObject.Find("Pause").GetComponent<Pause>().inMenu = false;
            GameObject.Find("Pause").GetComponent<Pause>().runningMenuTime = 0.0f;
        }
        if (GameObject.Find("GameManager") != null) {
            GameObject.Find("GameManager").GetComponent<GameManager>().ColorSettings();
            GameObject.Find("GameManager").GetComponent<GameManager>().reticle.SetActive(true);
        }
        if (GameObject.Find("Fade") != null) {
            GameObject.Find("Fade").GetComponent<Fade>().StartFadeIn(2f);
        }
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


        if (canvasObject != null) {
            canvasObject.SetActive(true);
        }
        if (GameObject.Find("Pause") != null) {
            GameObject.Find("Pause").GetComponent<Pause>().inMenu = false;
            GameObject.Find("Pause").GetComponent<Pause>().runningMenuTime = 0.0f;
        }
        if (GameObject.Find("GameManager") != null) {
            GameObject.Find("GameManager").GetComponent<GameManager>().ColorSettings();
            GameObject.Find("GameManager").GetComponent<GameManager>().reticle.SetActive(true);
        }
        if (GameObject.Find("Fade") != null) {
            GameObject.Find("Fade").GetComponent<Fade>().StartFadeIn(2f);
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
