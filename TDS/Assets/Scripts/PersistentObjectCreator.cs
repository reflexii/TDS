using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectCreator : MonoBehaviour {

    public GameObject gameManagerPrefab;
    public GameObject canvasPrefab;

	void Awake () {
        CreateGameManager();
        CreateGameCanvas();
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
}
