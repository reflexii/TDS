using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    public List<AudioClip> musicList;
    public bool lerpAudioAtStart = false;
    public float musicVolume = 100f;
    public float lerpTime = 3f;
    public string songNameToChangeInto = "";

    private AudioSource musicSource;
    private MainMenuScript mms;
    private float runningLerpTime;
    private float runningMusicStartTime;
    private bool startFadeIn = false;
    private bool startFadeOut = false;
    private bool startFadeOutIn = false;
    private bool firstPart = false;
    private bool secondPart = false;
    private float originalLerpTime;
    

	void Awake () {
        musicSource = GetComponent<AudioSource>();
        mms = GameObject.Find("MainMenuScript").GetComponent<MainMenuScript>();

        if (PlayerPrefs.HasKey("musicVolume")) {
            musicVolume = PlayerPrefs.GetInt("musicVolume") / 100f;
        } else {
            musicVolume = 1f;
        }

        musicSource.Play();

        if (lerpAudioAtStart) {
            FadeMusicIn();
        } else {
            musicSource.volume = musicVolume;
        }
	}
	
	void Update () {

        if (!startFadeIn && !startFadeOut && !startFadeOutIn) {
            musicSource.volume = musicVolume;
        }

        if (startFadeIn) {
            runningLerpTime += Time.deltaTime;

            musicSource.volume = (musicVolume / lerpTime) * runningLerpTime;

            if (musicSource.volume >= musicVolume) {
                musicSource.volume = musicVolume;
                startFadeIn = false;
            }
        }

        if (startFadeOut) {
            runningLerpTime -= Time.deltaTime;

            musicSource.volume = (musicVolume / lerpTime) * runningLerpTime;

            if (musicSource.volume <= 0f) {
                musicSource.volume = 0f;
                startFadeOut = false;
            }
        }

        if (startFadeOutIn) {
            if (firstPart) {
                runningLerpTime -= Time.deltaTime;

                musicSource.volume = (musicVolume / lerpTime) * runningLerpTime;

                if (musicSource.volume <= 0f) {
                    musicSource.volume = 0f;
                    firstPart = false;
                    secondPart = true;
                    lerpTime = 2f;

                    for (int i = 0; i < musicList.Count; i++) {
                        if (musicList[i].name.Equals(songNameToChangeInto)) {
                            musicSource.clip = musicList[i];
                            musicSource.Play();
                        }
                    }
                }
            }

            if (secondPart) {
                runningLerpTime += Time.deltaTime;

                musicSource.volume = (musicVolume / lerpTime) * runningLerpTime;

                if (musicSource.volume >= musicVolume) {
                    musicSource.volume = musicVolume;
                    lerpTime = originalLerpTime;
                    secondPart = false;
                    startFadeOutIn = false;
                }
            }
            

        }

        //TODO: REMOVE
        if (Input.GetKeyDown(KeyCode.Space)) {
            FadeOutAndInAndChangeSong("Jewel");
        }
	}

    public void FadeMusicIn() {
        startFadeIn = true;
        runningLerpTime = 0f;
    }

    public void FadeMusicOut() {
        startFadeOut = true;
        runningLerpTime = lerpTime;
    }

    public void FadeOutAndInAndChangeSong(string songName) {
        startFadeOutIn = true;
        songNameToChangeInto = songName;
        originalLerpTime = lerpTime;
        lerpTime = 1f;
        runningLerpTime = lerpTime;
        firstPart = true;
    }
}
