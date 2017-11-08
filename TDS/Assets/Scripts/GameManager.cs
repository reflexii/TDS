using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float loadSceneDelay;
    private float runningSceneTime;
    public bool playerIsDead = false;
    public GameObject player;

    private void Awake() {
    }

    public void Update() {
        if (playerIsDead) {
            runningSceneTime += Time.deltaTime;

            if (runningSceneTime >= loadSceneDelay) {
                LoadSameScene();
            }
        }
    }

    public void LoadSameScene() {
        runningSceneTime = 0f;
        //GetComponent<DialogManager>().reader.Close();
        player.GetComponent<PlayerMovement>().gameObject.SetActive(true);
        playerIsDead = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
