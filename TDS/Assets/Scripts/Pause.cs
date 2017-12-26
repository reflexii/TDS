using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

    public bool inMenu = false;
    public bool pauseMenuEnabled = false;
    public GameObject pauseMenuParent;
    public UnityEngine.UI.Button exit;
    public UnityEngine.UI.Button restart;
    public UnityEngine.UI.Button menu;
    public GameManager gm;
    public GameObject playerObject;


    private void Awake() {
        Cursor.lockState = CursorLockMode.Confined;
        pauseMenuParent = GameObject.Find("PauseMenu_parent");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pauseMenuParent.SetActive(false);
    }

    void Update () {
		
        if (!inMenu) {
            if (pauseMenuEnabled) {
                pauseMenuParent.SetActive(true);
                if (playerObject != null) {
                    playerObject.GetComponent<PlayerMovement>().gameIsPaused = true;
                }
                Time.timeScale = 0f;
                Cursor.visible = true;
            } else {
                pauseMenuParent.SetActive(false);
                Time.timeScale = 1f;
                Cursor.visible = false;
                if (playerObject != null) {
                    playerObject.GetComponent<PlayerMovement>().gameIsPaused = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                playerObject = GameObject.Find("Player");
                pauseMenuEnabled = !pauseMenuEnabled;
                gm.paused = pauseMenuEnabled;

                if (pauseMenuEnabled) {
                    gm.GetComponent<DialogManager>().ToggleDialogueUIOff();
                    GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().ToggleDeathScreenOff();
                }
            }
        }
	}

    public void RestartButton() {
        //add fadetoblack
        pauseMenuEnabled = false;
        gm.LoadSameScene();
    }

    public void ExitButton() {
        Application.Quit();
    }
    
    public void MenuButton() {
        gm.missionFailed = false;
        gm.playerIsDead = false;
        pauseMenuEnabled = false;
        pauseMenuParent.SetActive(false);
        Time.timeScale = 1f;
        inMenu = true;
        SceneManager.LoadScene("MainMenu");
    }
}
