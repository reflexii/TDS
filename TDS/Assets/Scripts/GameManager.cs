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

    private void Awake() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void Update() {
        if (playerIsDead || missionFailed) {
            runningSceneTime += Time.deltaTime;

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
        player.GetComponent<PlayerMovement>().gameObject.SetActive(true);
        Color temp = player.GetComponent<SpriteRenderer>().color;
        temp.a = 0f;
        player.GetComponent<SpriteRenderer>().color = temp;
        playerIsDead = false;
    }
}
