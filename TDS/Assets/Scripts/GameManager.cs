using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public Camera mainCamera;
    public GameObject Player;

    private void Awake() {
        Player = GameObject.Find("Player");
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
}
