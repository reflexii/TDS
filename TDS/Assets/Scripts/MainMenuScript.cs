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
    public bool startQuit = false;
    public float runningQuitTime = 0.0f;
    public Slider sliderSfx;
    public Slider sliderMusic;
    public Toggle sfxToggle;
    public Toggle musicToggle;
    public bool soundMuted;
    public bool musicMuted;
    public AudioSource buttonSound;
    public AudioSource gunshotSound;

    private bool prefLoaded = false;

    private void Start() {
        FirstTimeLaunch();
        LoadPlayerPreferences();
    }

    private void Update() {
        if (mainMenu) {
            UpdateContinueButton();
        }

        //Exit game after one second to play the fadetoblack animation before.
        if (startQuit) {
            runningQuitTime += Time.deltaTime;

            if (runningQuitTime >= 1f) {
                Application.Quit();
            }
        }
    }

    public void SaveSoundOptions() {
        if (PlayerPrefs.HasKey("soundVolume")) {
            sliderSfx = GameObject.Find("Slider").GetComponent<Slider>();
            PlayerPrefs.SetInt("soundVolume", (int)sliderSfx.value);
            sfxVolume = (int)sliderSfx.value;
        }
        if (PlayerPrefs.HasKey("musicVolume")) {
            sliderMusic = GameObject.Find("Slider (1)").GetComponent<Slider>();
            PlayerPrefs.SetInt("musicVolume", (int)sliderMusic.value);
            musicVolume = (int)sliderMusic.value;
        }
        if (PlayerPrefs.HasKey("musicToggle")) {
            musicToggle = GameObject.Find("Music").GetComponent<Toggle>();
            if (musicToggle.isOn) {
                PlayerPrefs.SetInt("musicToggle", 1);
                musicMuted = false;
            } else {
                PlayerPrefs.SetInt("musicToggle", 0);
                musicMuted = true;
            }
        }
        if (PlayerPrefs.HasKey("sfxToggle")) {
            sfxToggle = GameObject.Find("SFX").GetComponent<Toggle>();
            if (sfxToggle.isOn) {
                PlayerPrefs.SetInt("sfxToggle", 1);
                soundMuted = false;
            } else {
                PlayerPrefs.SetInt("sfxToggle", 0);
                soundMuted = true;
            }
        }

        if (!soundMuted) {
            buttonSound.volume = sfxVolume / 100f;
        } else {
            buttonSound.volume = 0f;
        }

        if (!musicMuted) {
            GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().musicVolume = musicVolume / 100f;
        } else {
            GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().musicVolume = 0f;
        }
        
    }

    public void LoadPlayerPreferences() {
        if (mainMenu) {
            if (PlayerPrefs.HasKey("currentLevel")) {
                currentLevel = PlayerPrefs.GetInt("currentLevel");
            }
            if (PlayerPrefs.HasKey("soundVolume")) {
                sliderSfx = GameObject.Find("Slider").GetComponent<Slider>();
                sfxVolume = PlayerPrefs.GetInt("soundVolume");
                sliderSfx.value = PlayerPrefs.GetInt("soundVolume");
            }
            if (PlayerPrefs.HasKey("musicVolume")) {
                sliderMusic = GameObject.Find("Slider (1)").GetComponent<Slider>();
                musicVolume = PlayerPrefs.GetInt("musicVolume");
                sliderMusic.value = PlayerPrefs.GetInt("musicVolume");
            }
            if (PlayerPrefs.HasKey("musicToggle")) {
                musicToggle = GameObject.Find("Music").GetComponent<Toggle>();
                if (PlayerPrefs.GetInt("musicToggle") == 1) {
                    musicToggle.isOn = true;
                    musicMuted = false;
                } else {
                    musicToggle.isOn = false;
                    musicMuted = true;
                }
            }
            if (PlayerPrefs.HasKey("sfxToggle")) {
                sfxToggle = GameObject.Find("SFX").GetComponent<Toggle>();
                if (PlayerPrefs.GetInt("sfxToggle") == 1) {
                    sfxToggle.isOn = true;
                    soundMuted = false;
                } else {
                    sfxToggle.isOn = false;
                    soundMuted = true;
                }
            }

            if (!musicMuted) {
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().musicVolume = musicVolume / 100f;
            } else {
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().musicVolume = 0f;
            }

            prefLoaded = true;
        }
    }

    public void OnChangeSoundValue() {
        if (prefLoaded) {
            if (!soundMuted) {
                if (sliderSfx != null) {
                    sfxVolume = sliderSfx.value;
                }
                gunshotSound.volume = sfxVolume / 100f;
                if (!gunshotSound.isPlaying) {
                    gunshotSound.Play();
                }
            } else {
                soundMuted = true;
            }
        } 
    }

    public void ChangeMusicVolume() {
        if (!musicMuted) {
            musicVolume = sliderMusic.value;
            GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().musicVolume = musicVolume / 100f;
        } else {
            GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().musicVolume = 0f;
        }
    }

    public void MusicToggle() {
        if (musicToggle.isOn) {
            musicMuted = false;
            musicVolume = sliderMusic.value;
            GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().musicVolume = musicVolume / 100f;
        } else {
            musicMuted = true;
            GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().musicVolume = 0f;
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
        if (!PlayerPrefs.HasKey("musicToggle")) {
            PlayerPrefs.SetInt("musicToggle", 1);
            musicMuted = false;
        }
        if (!PlayerPrefs.HasKey("sfxToggle")) {
            PlayerPrefs.SetInt("sfxToggle", 1);
            soundMuted = false;
        }
    }

    public void ClearKeys() {
        if (PlayerPrefs.HasKey("currentLevel")) {
            PlayerPrefs.SetInt("currentLevel", 1);
            currentLevel = PlayerPrefs.GetInt("currentLevel");
        }
    }

    public void UpdateContinueButton() {
        if (currentLevel >= 2) {
            continueButton.GetComponent<Button>().interactable = true;
        } else if (currentLevel < 2) {
            continueButton.GetComponent<Button>().interactable = false;
        }
    }

    public void ExitGame() {
        startQuit = true;
    }

    public void ButtonVolume() {
        if (!soundMuted) {
            buttonSound.volume = sfxVolume / 100f;
            gunshotSound.volume = sfxVolume / 100f;
        } else {
            buttonSound.volume = 0f;
            gunshotSound.volume = 0f;
        }
        
    }
}
