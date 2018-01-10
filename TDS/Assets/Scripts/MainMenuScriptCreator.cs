using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScriptCreator : MonoBehaviour {

    public GameObject mmsPrefab;

	void Awake () {
		if (GameObject.Find("MainMenuScript") == null) {
            GameObject g = Instantiate<GameObject>(mmsPrefab, Vector3.zero, Quaternion.identity);
            g.name = "MainMenuScript";
        }
	}
}
