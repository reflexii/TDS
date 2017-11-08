using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingEnemy : MonoBehaviour {

    public float waitTime;
    public float turnTime;
    public float amountToTurn;
    public float timeBeforeShooting;
    public GameObject gun;
    public bool shoot = false;
    public GameManager gameManager;
    public Quaternion savedRotation;

    private float currentWaitTime = 0f;
    private float currentTurnTime = 0f;
    private float shootingTime = 0f;

    private float currentZAxis;
    private bool idle = true;
    private GameObject player;
    


    private void Awake() {
        currentZAxis = transform.eulerAngles.z;
        player = GameObject.Find("Player");
    }

    void Update () {
        if (idle) {
            currentWaitTime += Time.deltaTime;

            if (currentWaitTime >= waitTime) {
                StartTurning(amountToTurn);
            }
        }

        if (shoot) {
            shootingTime += Time.deltaTime;
            if (shootingTime >= (timeBeforeShooting/1.3)) {
                //Rotate
                idle = false;
                Quaternion rotation = Quaternion.LookRotation
                (player.transform.position - transform.position, transform.TransformDirection(Vector3.up));
                transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
                currentWaitTime = 0f;
            }
            if (shootingTime >= timeBeforeShooting) {
                gun.GetComponent<Gun>().Shoot();
            }
        } else {
            idle = true;
            shootingTime = 0f;
        }
        
	}

    void StartTurning(float turnAmount) {
        currentTurnTime += Time.deltaTime;
        if (currentTurnTime <= turnTime) {
            transform.Rotate(new Vector3(0f, 0f, 1f) * Time.deltaTime * (turnAmount / turnTime));
        } else {
            currentZAxis += turnAmount;
            transform.eulerAngles = new Vector3(0f, 0f, currentZAxis);
            currentWaitTime = 0f;
            currentTurnTime = 0f;
        }
        
    }
}
