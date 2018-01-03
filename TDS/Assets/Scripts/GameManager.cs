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

    //make code to options for number next to volume slider + saving volume to playerprefs after pressing back

    public void NextLevelPreferences() {
        if (PlayerPrefs.HasKey("currentLevel")) {
            mms.currentLevel++;
            PlayerPrefs.SetInt("currentLevel", mms.currentLevel);
        }
    }

}
