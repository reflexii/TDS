using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

    public int currentLevel = 1;
    public float sfxVolume = 100f;
    public float musicVolume = 100f;
    public bool mainMenu = true;
    public GameObject continueButton;


	void Awake () {
        FirstTimeLaunch();
        LoadPlayerPreferences();
        Debug.Log(PlayerPrefs.GetInt("currentLevel"));
    }

    private void Update() {
        if (mainMenu) {
            UpdateContinueButton();
        }
    }

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

    public void ClearKeys() {
        if (PlayerPrefs.HasKey("currentLevel")) {
            Debug.Log("currentLevel: " + currentLevel);
            PlayerPrefs.SetInt("currentLevel", 1);
            currentLevel = PlayerPrefs.GetInt("currentLevel");
            Debug.Log("currentLevel: " + currentLevel);
        }
        if (PlayerPrefs.HasKey("soundVolume")) {
            PlayerPrefs.SetInt("soundVolume", 100);
        }
        if (PlayerPrefs.HasKey("musicVolume")) {
            PlayerPrefs.SetInt("musicVolume", 100);
        }
    }

    public void UpdateContinueButton() {
        if (currentLevel >= 2) {
            continueButton.GetComponent<Button>().interactable = true;
        } else {
            continueButton.GetComponent<Button>().interactable = false;
        }
    }
}
