using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneHandler : MonoBehaviour {

    private float runningAnyKeyTime = 0.0f;
    private GameManager gm;

	void Awake () {
        if (GameObject.Find("GameManager") != null) {
            Destroy(GameObject.Find("GameManager"));
        }
		if (GameObject.Find("CanvasGame") != null) {
            Destroy(GameObject.Find("CanvasGame"));
        }
        if (GameObject.Find("Reticle") != null) {
            Destroy(GameObject.Find("Reticle"));
        }
	}
	
	void Update () {
        runningAnyKeyTime += Time.deltaTime;

        //Time needed to get press any key to the screen on cutscene
        if (runningAnyKeyTime >= 28f) {
            if (Input.anyKeyDown) {
                Destroy(GameObject.Find("MainMenuScript"));
                Cursor.visible = true;
                SceneManager.LoadScene("MainMenu");

                //music change stuff here too!
            }
        }
	}
}
