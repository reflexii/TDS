using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist : MonoBehaviour {

    public enum CurrentStance { Moving, WaitingToMove, Alerted, Hiding};
    public bool ToggleQuestOnKill = false;
    public CurrentStance whatEnemyIsDoing;
    public Transform[] movementPositionList;
    public GameObject wayPointPrefab;
    public GameObject targetPrefab;
    public Transform targetParentObject;
    public AIPath ai;
    public float WaitTimeBeforeNextWaypoint = 2f;
    public GameObject alertButtonObjectToRunWhenAlerted;
    public float enemyHealth = 30f;
    public GameObject dieBloodBigPrefab;
    public GameObject dieBloodSmallPrefab;
    public GameObject corpse1;
    public GameObject corpse2;
    public bool standingEnemy = false;
    public float hideEulerAngles = 0f;
    public GameObject exclamationMarkPrefab;

    private int listLength;
    private Transform wayPointParentObject;
    private GameObject targetObject;
    private float runningWaitTime = 0f;
    private int positionIndex = 1;
    private GameObject player;
    private bool dead = false;
    private bool walking = false;
    private Animator animator;
    private bool doneOnce = false;
    private bool scared = false;

    //alerts
    private GameObject em;
    private GameObject symbolObject;

    void Awake () {
        ai = GetComponent<AIPath>();
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();

        if (GameObject.Find("WayPoints") == null) {
            GameObject g = new GameObject();
            g.name = "WayPoints";
        }

        wayPointParentObject = GameObject.Find("WayPoints").transform;

        if (GameObject.Find("EnemyTargets") == null) {
            GameObject g = new GameObject();
            g.name = "EnemyTargets";
        }
        targetParentObject = GameObject.Find("EnemyTargets").transform;

        if (movementPositionList.Length == 1) {
            standingEnemy = true;
        }

        CreateAlerts();
    }
	
    void Start() {
        listLength = movementPositionList.Length;
        GameObject target;

        if (movementPositionList[0] == null) {
            GameObject wp = Instantiate<GameObject>(wayPointPrefab, transform.position, Quaternion.identity, wayPointParentObject);
            wp.name = "Waypoint";
            movementPositionList[0] = wp.transform;

            if (!standingEnemy) {
                target = Instantiate<GameObject>(targetPrefab, movementPositionList[1].transform.position, Quaternion.identity, targetParentObject);
            } else {
                target = Instantiate<GameObject>(targetPrefab, movementPositionList[0].transform.position, Quaternion.identity, targetParentObject);
            }
            
        } else {
            target = Instantiate<GameObject>(targetPrefab, movementPositionList[0].transform.position, Quaternion.identity, targetParentObject);
        }

        target.GetComponent<TargetMovement>().enemyObject = gameObject;
        target.GetComponent<TargetMovement>().scientist = true;
        target.GetComponent<TargetMovement>().movingEnemy = false;
        target.name = "Scientist Target";
        ai.target = target.transform;
        targetObject = target;

        if (standingEnemy) {
            targetObject.transform.position = transform.position;
            whatEnemyIsDoing = CurrentStance.WaitingToMove;
        }
    }

    void CreateAlerts() {
        if (GameObject.Find("Symbols") == null) {
            symbolObject = new GameObject();
            symbolObject.name = "Symbols";
        } else {
            symbolObject = GameObject.Find("Symbols");
        }
        em = Instantiate<GameObject>(exclamationMarkPrefab, transform.position + new Vector3(0f, 0.9f, 0f), Quaternion.identity);
        em.transform.parent = symbolObject.transform;
        em.GetComponent<SpriteRenderer>().enabled = false;
    }

    void UpdateAlerts() {
        em.transform.position = transform.position + new Vector3(0f, 0.9f, 0f);
    }

    void Animations() {
        animator.SetBool("Walking", walking);
        animator.SetBool("Scared", scared);
    }

	void Update () {

        UpdateAlerts();
        Animations();

		switch (whatEnemyIsDoing) {
            case CurrentStance.Moving:
                em.GetComponent<SpriteRenderer>().enabled = false;
                walking = true;
                break;
            case CurrentStance.WaitingToMove:
                if (!standingEnemy) {
                    runningWaitTime += Time.deltaTime;
                    walking = false;
                    MoveTarget();
                }
                em.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case CurrentStance.Alerted:
                walking = true;
                ai.speed = 6f;
                animator.speed = 2f;
                targetObject.transform.position = alertButtonObjectToRunWhenAlerted.transform.position;
                em.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case CurrentStance.Hiding:
                transform.eulerAngles = new Vector3(0f, 0f, hideEulerAngles);
                animator.speed = 3f;
                walking = false;
                scared = true;
                ai.canMove = false;
                em.GetComponent<SpriteRenderer>().enabled = true;
                break;
        }
	}

    void MoveTarget() {
        if (runningWaitTime >= WaitTimeBeforeNextWaypoint) {
            positionIndex++;
            if (positionIndex <= listLength - 1) {
                targetObject.transform.position = movementPositionList[positionIndex].position;
            } else {
                positionIndex = 0;
                targetObject.transform.position = movementPositionList[positionIndex].position;
            }

            runningWaitTime = 0f;
            whatEnemyIsDoing = CurrentStance.Moving;
        }
    }

    void RandomizeAndSpawnCorpse() {
        int randomize = Random.Range(0, 2);
        GameObject chosenCorpse;

        if (randomize == 0) {
            chosenCorpse = corpse1;
        } else {
            chosenCorpse = corpse2;
        }

        Instantiate<GameObject>(chosenCorpse, transform.position, Quaternion.identity);
    }

    void Die() {
        dead = true;

        RandomizeAndSpawnCorpse();

        Instantiate<GameObject>(dieBloodBigPrefab, transform.position, Quaternion.identity);
        Instantiate<GameObject>(dieBloodSmallPrefab, transform.position, Quaternion.identity);

        if (movementPositionList.Length > 0) {
            for (int i = 0; i < movementPositionList.Length; i++) {
                Destroy(movementPositionList[i].gameObject, 0f);
            }
        }

        if (ToggleQuestOnKill && !doneOnce) {
            GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().NextObjective();
            doneOnce = true;
        }

        Destroy(em);
        Destroy(targetObject, 0f);
        Destroy(gameObject, 0f);
    }

    public void DamageEnemy(float damageValue) {
        Instantiate<GameObject>(dieBloodSmallPrefab, transform.position, Quaternion.identity);
        whatEnemyIsDoing = CurrentStance.Alerted;

        enemyHealth -= damageValue;

        if (enemyHealth > 0f) {
            Collider2D[] shootingSoundWave = Physics2D.OverlapCircleAll(transform.position, 5f, 1 << LayerMask.NameToLayer("Enemy"));

            if (shootingSoundWave != null) {
                for (int i = 0; i < shootingSoundWave.Length; i++) {
                    if (shootingSoundWave[i] != null) {
                        if (shootingSoundWave[i].GetComponent<MovingEnemy>() != null) {
                            if (shootingSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.Moving ||
                            shootingSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.WaitingToMove ||
                            shootingSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.SearchingPlayer &&
                            shootingSoundWave[i].GetComponent<MovingEnemy>().GetRunningTime() >= 3f) {
                                shootingSoundWave[i].GetComponent<MovingEnemy>().playerSearchPosition = player.transform.position;
                                shootingSoundWave[i].GetComponent<MovingEnemy>().whatEnemyIsDoing = MovingEnemy.CurrentStance.SearchingPlayer;
                            }
                        }
                    }
                }
            }
        }

        if (enemyHealth <= 0f && !dead) {
            Die();
        }
    }

    public float GetRunningTime() {
        return runningWaitTime;
    }
}
