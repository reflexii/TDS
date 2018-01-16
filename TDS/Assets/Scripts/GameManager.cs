using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float loadSceneDelay;
    public bool playerIsDead = false;
    public bool missionFailed = false;
    public GameObject player;
    public MainMenuScript mms;
    public GameObject mmsPrefab;
    public bool paused = false;
    public GameObject reticle;

    private float runningSceneTime;
    private bool doneOnce = false;
    private bool toggleMapChange = false;
    private float value = 1f;
    private UnityEngine.PostProcessing.PostProcessingBehaviour pp;
    private UnityEngine.PostProcessing.ColorGradingModel.Settings profile;

    private void Awake() {

        ColorSettings();

        if (GameObject.Find("MainMenuScript") != null) {
            mms = GameObject.Find("MainMenuScript").GetComponent<MainMenuScript>();
        } else {
            GameObject g = Instantiate<GameObject>(mmsPrefab, Vector3.zero, Quaternion.identity);
            mms = g.GetComponent<MainMenuScript>();
            g.name = "MainMenuScript";
            g.GetComponent<MainMenuScript>().mainMenu = false;
        }
    }

    public void ColorSettings() {
        pp = GameObject.Find("Main Camera").GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>();
        profile = pp.profile.colorGrading.settings;
        value = 1f;
        profile.basic.saturation = value;
        profile.tonemapping.neutralWhiteIn = 10f;
        pp.profile.colorGrading.settings = profile;
    }

    public void Update() {

        if (playerIsDead || missionFailed) {
            runningSceneTime += Time.deltaTime;

            GetComponent<DialogManager>().ToggleDialogueUIOff();

            if (value >= 0) {
                value -= Time.deltaTime;
            }
            profile.basic.saturation = value;

            if (value <= 0f) {
                profile.tonemapping.neutralWhiteIn = 1f;
            } else {
                profile.tonemapping.neutralWhiteIn = (value * 10f);
            }
            pp.profile.colorGrading.settings = profile;

            if (runningSceneTime >= 1f && !paused) {
                if (Input.GetKeyDown(KeyCode.Mouse0) && !doneOnce) {
                    GameObject.Find("Fade").GetComponent<Fade>().StartFadeOut(1f);
                    GameObject.Find("Pause").GetComponent<Pause>().runningMenuTime = 0f;
                    runningSceneTime = 1f;
                    toggleMapChange = true;
                    doneOnce = true;
                }
                if (runningSceneTime >= 2f && toggleMapChange) {
                    toggleMapChange = false;
                    doneOnce = false;
                    reticle.SetActive(true);
                    LoadSameScene();
                }  
            }
        }
    }

    public void LoadSameScene() {
        runningSceneTime = 0f;
        missionFailed = false;
        GameObject.Find("Fade").GetComponent<Fade>().StartFadeIn(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        value = 1f;
        profile.basic.saturation = value;
        profile.tonemapping.neutralWhiteIn = 10f;
        pp.profile.colorGrading.settings = profile;
        player.GetComponent<PlayerMovement>().gameObject.SetActive(true);
        Color temp = player.GetComponent<SpriteRenderer>().color;
        temp.a = 0f;
        player.GetComponent<SpriteRenderer>().color = temp;
        playerIsDead = false;
    }

    public void NextLevelPreferences(string mapName) {
        if (PlayerPrefs.HasKey("currentLevel")) {
            switch (mapName) {
                case "1stmap":
                    mms.currentLevel = 1;
                    break;
                case "layerwarehouse":
                    mms.currentLevel = 2;
                    break;
                case "backdoorgrenade":
                    mms.currentLevel = 3;
                    break;
                case "gaslab":
                    mms.currentLevel = 4;
                    break;
                case "glassfloor":
                    mms.currentLevel = 5;
                    break;
                case "gauntlet":
                    mms.currentLevel = 6;
                    break;
                case "corridors":
                    mms.currentLevel = 7;
                    break;
                case "alertlab":
                    mms.currentLevel = 8;
                    break;
                case "redalert":
                    mms.currentLevel = 9;
                    break;
                case "switchhell":
                    mms.currentLevel = 10;
                    break;
                case "act1end":
                    mms.currentLevel = 11;
                    break;
                case "escaperework":
                    mms.currentLevel = 12;
                    break;
                case "longhall":
                    mms.currentLevel = 13;
                    break;
                case "prison":
                    mms.currentLevel = 14;
                    break;
                case "trapped":
                    mms.currentLevel = 15;
                    break;
                case "prison2":
                    mms.currentLevel = 16;
                    break;
                case "trapped2":
                    mms.currentLevel = 17;
                    break;
                case "killzone":
                    mms.currentLevel = 18;
                    break;
                case "madlab":
                    mms.currentLevel = 19;
                    break;
                case "shitstorm":
                    mms.currentLevel = 20;
                    break;
                case "boss":
                    mms.currentLevel = 21;
                    break;
                default:
                    mms.currentLevel = 21;
                    break;
                
            }

            PlayerPrefs.SetInt("currentLevel", mms.currentLevel);
        }
    }

}
