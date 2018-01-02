using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour {

    public float fadeTime = 1f;
    public bool startBlack = false;
    public float fadeValue = 0f;

    private bool startFadeOut = false;
    private bool startFadeIn = false;

    public List<GameObject> fadeList;

	void Awake () {

        if (startBlack) {
            fadeValue = 1f;
            StartFadeIn();
        } else {
            fadeValue = 0f;
        }

        Color newColor = new Color(0f, 0f, 0f, fadeValue);
        GetComponent<Image>().color = newColor;

    }
	
	void Update () {
        if (startFadeOut) {
            FadeOut();
        }

        if (startFadeIn) {
            FadeIn();
            
        }
	}

    //0 -> 1
    void FadeOut() {

        if (fadeValue < 1f) {
            fadeValue += Time.deltaTime/fadeTime;
        } else {
            fadeValue = 1f;
            startFadeOut = false;
        }

        Color newColor = new Color(0f, 0f, 0f, fadeValue);
        GetComponent<Image>().color = newColor;
        
    }

    //1 -> 0
    void FadeIn() {
        if (fadeValue > 0f) {
            fadeValue -= Time.deltaTime / fadeTime;
        } else {
            fadeValue = 0f;
            startFadeIn = false;
        }

        Color newColor = new Color(0f, 0f, 0f, fadeValue);
        GetComponent<Image>().color = newColor;
    }

    public void StartFadeOut(float fadeOutTime = 2f) {
        fadeTime = fadeOutTime;
        fadeValue = 0f;
        startFadeOut = true;
        startFadeIn = false;
    }

    public void StartFadeIn(float fadeInTime = 2f) {
        fadeTime = fadeInTime;
        fadeValue = 1f;
        startFadeIn = true;
        startFadeOut = false;
    }
}
