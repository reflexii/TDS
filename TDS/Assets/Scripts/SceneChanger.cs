using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour {

    public string sceneName = "";
    public float timeBeforeSwitching;
    public bool mainMenu = false;
    public float fadeTime = 0.5f;
    public bool changeMusic = false;
    public string musicName = "";

    private ObjectiveManager om;
    private GameManager gm;
    private bool scenesChanging = false;
    private float runningSwitchTime = 0.0f;
    private MainMenuScript mms;
    private bool fadedOnce = false;
    private GameObject canvasObject;
    private GameObject playerObject;

    private void Start() {
        if (!mainMenu) {
            om = GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>();
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        } else {
            mms = GameObject.Find("MainMenuScript").GetComponent<MainMenuScript>();

            if (GameObject.Find("Fade") != null) {
                GameObject.Find("Fade").GetComponent<Fade>().fadeValue = 1f;
                GameObject.Find("Fade").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            }
            if (GameObject.Find("CanvasGame") != null) {
                Destroy(GameObject.Find("CanvasGame"));
            }
            if (GameObject.Find("GameManager") != null) {
                Destroy(GameObject.Find("GameManager"));
            }
            if (GameObject.Find("Reticle") != null) {
                Destroy(GameObject.Find("Reticle"));
            }
            if (GameObject.Find("PooledObjects")) {
                Destroy(GameObject.Find("PooledObjects"));
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
            playerObject = collision.gameObject;
            playerObject.GetComponent<PlayerMovement>().playerCantBeDamaged = true;
        }
    }

    void ChangeScene() {

        GameObject.Find("MissionComplete").transform.GetChild(0).GetComponent<Text>().color = new Color(0f, 0f, 0f, 1f);
        GameObject.Find("MissionComplete").transform.GetChild(1).GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);

        if (!fadedOnce && runningSwitchTime >= fadeTime) {
            GameObject.Find("Fade").GetComponent<Fade>().StartFadeOut();
            fadedOnce = true;
            if (changeMusic && GameObject.Find("MusicPlayer") != null) {
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong(musicName);
            }
        }
        
        //Mission complete

        runningSwitchTime += Time.deltaTime;

        if (runningSwitchTime >= timeBeforeSwitching) {
            gm.NextLevelPreferences(sceneName);
            StopAllSounds();

            GameObject.Find("Fade").GetComponent<Fade>().StartFadeIn();
            GameObject.Find("MissionComplete").transform.GetChild(0).GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
            GameObject.Find("MissionComplete").transform.GetChild(1).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
            playerObject.GetComponent<PlayerMovement>().playerCantBeDamaged = false;
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
            GameObject.Find("GameManager").GetComponent<GameManager>().mms = GameObject.Find("MainMenuScript").GetComponent<MainMenuScript>();

        }
        if (GameObject.Find("Fade") != null) {
            GameObject.Find("Fade").GetComponent<Fade>().StartFadeIn(2f);
        }

        GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("Jewel");

        SceneManager.LoadScene("1stmap");
    }

    public void ContinueLevel() {
        string name = "";
        mms.mainMenu = false;

        switch (mms.currentLevel) {
            case 1:
                name = "1stmap";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("Jewel");
                break;
            case 2:
                name = "layerwarehouse";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("Jewel");
                break;
            case 3:
                name = "backdoorgrenade";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("Jewel");
                break;
            case 4:
                name = "gaslab";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("moonlit");
                break;
            case 5:
                name = "glassfloor";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("moonlit");
                break;
            case 6:
                name = "gauntlet";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("fourth");
                break;
            case 7:
                name = "corridors";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("moonlit");
                break;
            case 8:
                name = "alertlab";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("monster");
                break;
            case 9:
                name = "redalert";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("monster");
                break;
            case 10:
                name = "switchhell";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("monster");
                break;
            case 11:
                name = "act1end";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("monster");
                break;
            case 12:
                name = "escaperework";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("Sewer");
                break;
            case 13:
                name = "longhall";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("Sewer");
                break;
            case 14:
                name = "prison";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("Sector");
                break;
            case 15:
                name = "trapped";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("Sector");
                break;
            case 16:
                name = "prison2";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("Sector");
                break;
            case 17:
                name = "trapped2";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("moonbase");
                break;
            case 18:
                name = "killzone";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("fourth");
                break;
            case 19:
                name = "madlab";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("moonbase");
                break;
            case 20:
                name = "shitstorm";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("moonbase");
                break;
            case 21:
                name = "boss";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("space");
                break;
            default:
                name = "boss";
                GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().FadeOutAndInAndChangeSong("space");
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
            GameObject.Find("GameManager").GetComponent<GameManager>().mms = GameObject.Find("MainMenuScript").GetComponent<MainMenuScript>();
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
