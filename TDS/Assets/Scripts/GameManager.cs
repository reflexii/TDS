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

    private UnityEngine.PostProcessing.PostProcessingBehaviour pp;
    private UnityEngine.PostProcessing.ColorGradingModel.Settings profile;

    private void Awake() {
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
}
