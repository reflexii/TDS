using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

    public string sceneName = "";
    public float timeBeforeSwitching;

    private ObjectiveManager om;
    private bool scenesChanging = false;
    private float runningSwitchTime = 0.0f;

    private void Start() {
        om = GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>();
    }

    private void Update() {
        if (scenesChanging) {
            ChangeScene();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (om.objectivesComplete && collision.gameObject.layer == LayerMask.NameToLayer("Player1")) {
            scenesChanging = true;
        }
    }

    void ChangeScene() {
        runningSwitchTime += Time.deltaTime;

        if (runningSwitchTime >= timeBeforeSwitching) {
            SceneManager.LoadScene(sceneName);
        }
    }
}
