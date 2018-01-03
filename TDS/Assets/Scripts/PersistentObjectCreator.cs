using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectCreator : MonoBehaviour {

    public GameObject gameManagerPrefab;
    public GameObject canvasPrefab;
    public GameObject reticlePrefab;
    public GameObject pausePrefab;
    public GameObject eventPrefab;

	void Awake () {
        CreateGameManager();
        CreateGameCanvas();
        CreateEventSystem();
        CreateTargetReticle();
        CreatePause();
	}

    void CreateGameManager() {
        if (GameObject.Find("GameManager") == null) {
            GameObject g = Instantiate<GameObject>(gameManagerPrefab);
            g.name = "GameManager";
        }
    }

    void CreateGameCanvas() {
        if (GameObject.Find("CanvasGame") == null) {
            GameObject g = Instantiate<GameObject>(canvasPrefab);
            g.name = "CanvasGame";
        }
    }

    void CreateTargetReticle() {
        if (GameObject.Find("Reticle") == null) {
            GameObject g = Instantiate<GameObject>(reticlePrefab);
            g.name = "Reticle";
            GameObject.Find("GameManager").GetComponent<GameManager>().reticle = g;
        }
    }

    void CreateEventSystem() {
        if (GameObject.Find("EventSystem") == null) {
            GameObject g = Instantiate<GameObject>(eventPrefab);
            g.name = "EventSystem";
        }
    }

    void CreatePause() {
        if (GameObject.Find("Pause") == null) {
            GameObject g = Instantiate<GameObject>(pausePrefab);
            g.name = "Pause";
        }
    }
}
