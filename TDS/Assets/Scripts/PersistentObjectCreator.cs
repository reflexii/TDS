using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectCreator : MonoBehaviour {

    public GameObject gameManagerPrefab;
    public GameObject canvasPrefab;
    public GameObject reticlePrefab;

	void Awake () {
        CreateGameManager();
        CreateGameCanvas();
        CreateTargetReticle();
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
        }
    }
}
