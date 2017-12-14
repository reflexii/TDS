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

    private void Start() {
        if (!mainMenu) {
            om = GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>();
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            SceneManager.LoadScene(sceneName);
        }
    }

    public void ChangeToFirstMap() {
        SceneManager.LoadScene("1stmap");
    }
}
