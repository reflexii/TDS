using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float loadSceneDelay;
    private float runningSceneTime;
    public bool playerIsDead = false;
    public bool missionFailed = false;
    public GameObject player;
    private float value = 1f;
    public int currentLevel = 1;
    public int sfxVolume = 100;
    public int musicVolume = 100;

    private UnityEngine.PostProcessing.PostProcessingBehaviour pp;
    private UnityEngine.PostProcessing.ColorGradingModel.Settings profile;

    private void Awake() {

        FirstTimeLaunch();
        LoadPlayerPreferences();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        pp = GameObject.Find("Main Camera").GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>();
        profile = pp.profile.colorGrading.settings;
        profile.basic.saturation = value;
        profile.tonemapping.neutralWhiteIn = 10f;
        pp.profile.colorGrading.settings = profile;
    }

    public void Update() {
        if (playerIsDead || missionFailed) {
            runningSceneTime += Time.deltaTime;

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

            if (runningSceneTime >= loadSceneDelay || runningSceneTime >= 2f && Input.GetKeyDown(KeyCode.Mouse0)) {
                LoadSameScene();
            }
        }

        //REMOVE
        if (Input.GetKeyDown(KeyCode.Space)) {
            PlayerPrefs.SetInt("currentLevel", (PlayerPrefs.GetInt("currentLevel") + 1));
        }

        Debug.Log(currentLevel);
    }

    public void LoadSameScene() {
        runningSceneTime = 0f;
        missionFailed = false;
        //GetComponent<DialogManager>().reader.Close();
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
    public void LoadPlayerPreferences() {
        if (PlayerPrefs.HasKey("currentLevel")) {
            currentLevel = PlayerPrefs.GetInt("currentLevel");
        }
        if (PlayerPrefs.HasKey("soundVolume")) {
            sfxVolume = PlayerPrefs.GetInt("soundVolume");
        }
        if (PlayerPrefs.HasKey("musicVolume")) {
            musicVolume = PlayerPrefs.GetInt("musicVolume");
        }
    }

    public void FirstTimeLaunch() {
        if (!PlayerPrefs.HasKey("currentLevel")) {
            PlayerPrefs.SetInt("currentLevel", 1);
        }
        if (!PlayerPrefs.HasKey("soundVolume")) {
            PlayerPrefs.SetInt("soundVolume", 100);
        }
        if (!PlayerPrefs.HasKey("musicVolume")) {
            PlayerPrefs.SetInt("musicVolume", 100);
        }
    }

    public void NextLevelPreferences() {
        if (PlayerPrefs.HasKey("currentLevel")) {
            currentLevel++;
            PlayerPrefs.SetInt("currentLevel", currentLevel);
        }
    }

}
