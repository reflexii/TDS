using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert : MonoBehaviour {

    private GameObject alertEnemySpawnPosition;
    private GameObject primaryAlertWaypoints;
    private GameObject secondaryAlertWaypoints;
    private float runningEnemySpawnTime = 0f;
    private int currentEnemiesSpawned = 0;
    private GameObject[] primaryWaypoints;
    private GameObject[] secondaryWaypoints;
    public List<GameObject> primaryWpList;
    private bool sameWaypoint = true;
    private int randomSecondaryOne;
    private int randomSecondaryTwo;
    private ObjectiveManager om;
    private bool doneOnce = false;
    private GameManager gm;

    public GameObject[] enemySpawnPositions;
    public int numberOfEnemiesSpawned;
    public float timeBetweenEnemySpawns;
    public GameObject enemyPrefab;
    public bool AlertOn = false;
    public Gun.Weapons[] weaponsAllowed;

	void Awake () {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        om = GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>();
        primaryAlertWaypoints = transform.Find("AlertWayPoints").Find("Primary").gameObject;
        secondaryAlertWaypoints = transform.Find("AlertWayPoints").Find("Secondary").gameObject;
        primaryWpList = new List<GameObject>();
        GenerateWayPointList();
	}

    void GenerateWayPointList() {
        int children = primaryAlertWaypoints.transform.childCount;
        int children2 = secondaryAlertWaypoints.transform.childCount;

        if (children == 0 || children2 == 0) {
            Debug.Log("You need both primary and secondary waypoints.");
        } else {
            primaryWaypoints = new GameObject[children];
            for (int i = 0; i < children; i++) {
                primaryWaypoints[i] = primaryAlertWaypoints.transform.GetChild(i).gameObject;
                primaryWpList.Add(primaryAlertWaypoints.transform.GetChild(i).gameObject);
            }

            secondaryWaypoints = new GameObject[children2];
            for (int i = 0; i < children2; i++) {
                secondaryWaypoints[i] = secondaryAlertWaypoints.transform.GetChild(i).gameObject;
            }
        }
    }

    public Gun.Weapons RandomizeWeapon() {
        int length = weaponsAllowed.Length;
        int random = Random.Range(0, length);
        return weaponsAllowed[random];
    }

    public Vector3 RandomizeSpawnLocation() {
        int length = enemySpawnPositions.Length;
        int random = Random.Range(0, length);

        return enemySpawnPositions[random].transform.position;
    }

    void SpawnEnemy() {
        GameObject enemy = Instantiate<GameObject>(enemyPrefab, RandomizeSpawnLocation(), Quaternion.identity);
        om.enemiesLeft++;
        enemy.transform.Find("Gun").GetComponent<Gun>().weaponInUse = RandomizeWeapon();
        enemy.transform.Find("Gun").GetComponent<Gun>().WeaponPreparation();
        enemy.GetComponent<MovingEnemy>().destroyWaypoints = false;
        //assign primary waypoint, where the enemy travels first
        //if all primaries are already in use, randomize
        if (primaryWpList.Count >= 1) {
            int randomPrimary = Random.Range(0, primaryWpList.Count);
            enemy.GetComponent<MovingEnemy>().movementPositionList[0] = primaryWpList[randomPrimary].transform;
            primaryWpList.Remove(primaryWpList[randomPrimary].gameObject);
        } else {
            int randomPrimary = Random.Range(0, primaryWaypoints.Length);
            enemy.GetComponent<MovingEnemy>().movementPositionList[0] = primaryWaypoints[randomPrimary].transform;
        }
        //assign secondary waypoints at random, two per enemy
        if (secondaryWaypoints.Length > 1) {

            if (sameWaypoint) {
                SecondaryRandomizer();
            }

            enemy.GetComponent<MovingEnemy>().movementPositionList[1] = secondaryWaypoints[randomSecondaryOne].transform;
            enemy.GetComponent<MovingEnemy>().movementPositionList[2] = secondaryWaypoints[randomSecondaryTwo].transform;
            sameWaypoint = true;
        } else {
            Debug.Log("Not enough secondary waypoints!");
        }
    }

    private void SecondaryRandomizer() {
        randomSecondaryOne = Random.Range(0, secondaryWaypoints.Length);
        randomSecondaryTwo = Random.Range(0, secondaryWaypoints.Length);

        if (randomSecondaryOne == randomSecondaryTwo) {
            sameWaypoint = true;
            SecondaryRandomizer();
        } else {
            sameWaypoint = false;
        }
    }

	void Update () {
        if (AlertOn) {
            runningEnemySpawnTime += Time.deltaTime;

            if (!doneOnce) {
                gm.GetComponent<SoundManager>().PlaySound("Alert", false);
                doneOnce = true;
            }

            if (currentEnemiesSpawned < numberOfEnemiesSpawned) {
                if (runningEnemySpawnTime >= timeBetweenEnemySpawns) {
                    SpawnEnemy();
                    runningEnemySpawnTime = 0f;
                    currentEnemiesSpawned++;
                }
            }
        }
	}
}
