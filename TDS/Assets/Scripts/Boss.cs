using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour {

    public enum CurrentStance { Shooting, Waiting, Loading, Moving, ShootingAtComputer };
    public CurrentStance whatEnemyIsDoing;
    public GameManager gameManager;
    public GameObject gun;
    public AIPath ai;
    public Transform targetParentObject;
    public GameObject exclamationMarkPrefab;
    public GameObject targetPrefab;
    public Vector3 savedPlayerPosition;
    public Vector3 newPlayerPosition;
    public bool walking = false;
    public bool shooting = false;
    public Transform[] movementPositionList;
    public GameObject wayPointPrefab;
    public float WaitTimeBeforeNextWaypoint = 2f;
    public int turnIndex = 0;
    public GameObject dieBloodBigPrefab;
    public GameObject dieBloodSmallPrefab;
    public float enemyHealth = 60f;
    public Text bossHpText;
    public GameObject buttonTarget;

    private GameObject player;
    private GameObject symbolObject;
    private Animator animator;
    private GameObject em;
    private GameObject targetObject;
    private bool doneOnce = false;
    private bool doneOnce2 = false;
    private float runningShootTime = 0.0f;
    private float runningComputerTime = 0f;
    private Vector3 direction;
    private float distanceBetweenPositions;
    private Transform wayPointParentObject;
    private int listLength;
    private float runningWaitTime = 0f;
    private float runningTurnTime = 3f;
    private int positionIndex = 1;
    private bool didOnce = false;
    private float randomize = 0f;
    private float waitRotateSpeed = 4f;
    private int startingDirection = 1;
    private float standardMovement = 115f;
    private bool dead = false;
    private float maxHealth;

    void Awake () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        gun = transform.Find("Gun").gameObject;
        ai = GetComponent<AIPath>();
        gun.GetComponent<Gun>().playerOwned = false;
        maxHealth = enemyHealth;

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
        animator = GetComponent<Animator>();

        CreateAlerts();

    }

    private void Start() {

        listLength = movementPositionList.Length;

        GameObject target;

        if (movementPositionList[0] == null) {
            GameObject wp = Instantiate<GameObject>(wayPointPrefab, transform.position, Quaternion.identity, wayPointParentObject);
            wp.name = "Waypoint";
            movementPositionList[0] = wp.transform;
            target = Instantiate<GameObject>(targetPrefab, movementPositionList[1].transform.position, Quaternion.identity, targetParentObject);
        } else {
            target = Instantiate<GameObject>(targetPrefab, movementPositionList[0].transform.position, Quaternion.identity, targetParentObject);
        }


        target.GetComponent<TargetMovement>().enemyObject = gameObject;
        target.GetComponent<TargetMovement>().scientist = false;
        target.GetComponent<TargetMovement>().movingEnemy = false;
        target.GetComponent<TargetMovement>().boss = true;
        target.name = "Boss target";
        ai.target = target.transform;
        targetObject = target;
        
    }

    void HealthUpdate() {
        bossHpText.text = enemyHealth + "/" + maxHealth;
    }

    void Update () {
        Animations();
        UpdateAlerts();
        HealthUpdate();

        switch (whatEnemyIsDoing) {
            case CurrentStance.Waiting:
                doneOnce = false;
                doneOnce2 = false;
                shooting = false;
                walking = false;
                ai.canMove = true;
                ai.canSearch = true;

                //Turning around, preparing to walk around

                runningWaitTime += Time.deltaTime;
                runningTurnTime += Time.deltaTime;
                MoveTarget();

                if (!didOnce) {
                    randomize = Random.Range(0.75f, 1.25f);
                    didOnce = true;
                }

                if (runningTurnTime <= 2.5f && runningTurnTime >= randomize) {

                    waitRotateSpeed -= (Time.deltaTime * 7f);
                    if (waitRotateSpeed <= 0f) {
                        waitRotateSpeed = 0f;
                    }

                    transform.Rotate(new Vector3(0f, 0f, 1f) * Time.deltaTime * (75f * startingDirection * waitRotateSpeed));


                } else if (runningTurnTime > 2.5f) {
                    runningTurnTime = 0f;
                    waitRotateSpeed = 4f;
                    didOnce = false;
                    if (turnIndex % 4 == 1 || turnIndex % 4 == 2) {
                        startingDirection = -1;
                    } else {
                        startingDirection = 1;
                    }

                    turnIndex++;
                }
                    break;
            case CurrentStance.Moving:
                //Moving from one waypoint to another, afterwards goes back to CurrentStance.Waiting
                walking = true;
                ai.canMove = true;
                ai.canSearch = true;
                turnIndex = 0;
                runningWaitTime = 0f;
                break;
            case CurrentStance.ShootingAtComputer:

                runningComputerTime += Time.deltaTime;

                if (runningComputerTime < 0.5f) {
                    ai.canMove = true;
                    ai.canSearch = true;
                    shooting = false;
                    walking = false;
                }
                else if (gun.GetComponent<Gun>().runningCooldown > gun.GetComponent<Gun>().minigunCooldown && gun.GetComponent<Gun>().minigunBulletsShot <= 260f && runningComputerTime >= 0.5f) {
                    gun.GetComponent<Gun>().BossShoot(buttonTarget.transform.position - transform.position);
                    transform.up = buttonTarget.transform.position - transform.position;
                    ai.canMove = false;
                    ai.canSearch = false;
                    shooting = true;
                    walking = false;
                } else if (gun.GetComponent<Gun>().minigunBulletsShot > 260f) {
                    shooting = false;
                    transform.Find("BulletSpawnPoint/Muzzle").GetComponent<SpriteRenderer>().enabled = false;
                    runningShootTime = 0f;
                    ai.canMove = true;
                    ai.canSearch = true;
                    walking = false;
                    runningComputerTime = 0f;
                    whatEnemyIsDoing = CurrentStance.Loading;
                }
                break;
            case CurrentStance.Shooting:
                walking = false;

                runningShootTime += Time.deltaTime;

                //shoot 20 bullets towards player's old position, start rotating to the side the player was moving

                if (runningShootTime < 0.25f) {
                    if (!doneOnce) {
                        transform.up = player.transform.position - transform.position;
                        savedPlayerPosition = player.transform.position;
                        doneOnce = true;
                    }
                    shooting = false;
                    ai.canMove = false;
                    ai.canSearch = false;
                }
                //shoot 20 bullets towards saved player position
                if (runningShootTime >= 0.25f && gun.GetComponent<Gun>().minigunBulletsShot <= 40 && gun.GetComponent<Gun>().runningCooldown > gun.GetComponent<Gun>().minigunCooldown) {
                    gun.GetComponent<Gun>().BossShoot(savedPlayerPosition - transform.position);
                    ai.canMove = false;
                    ai.canSearch = false;
                    shooting = true;
                } else if (gun.GetComponent<Gun>().minigunBulletsShot > 40 && gun.GetComponent<Gun>().minigunBulletsShot < 200 && gun.GetComponent<Gun>().runningCooldown > gun.GetComponent<Gun>().minigunCooldown) {
                    //if (!doneOnce2) {
                        newPlayerPosition = player.transform.position;
                        //doneOnce2 = true;
                        direction = (newPlayerPosition - savedPlayerPosition).normalized;
                        distanceBetweenPositions = Vector3.Distance(newPlayerPosition, savedPlayerPosition);
                    //}

                    shooting = true;
                    ai.canMove = false;
                    ai.canSearch = false;
                    savedPlayerPosition += (direction / standardMovement * distanceBetweenPositions*10f);

                    if (standardMovement >= 6f) {
                        standardMovement -= 1f;
                    }
                    transform.up = savedPlayerPosition - transform.position;
                    gun.GetComponent<Gun>().BossShoot(savedPlayerPosition - transform.position);
                } else if (gun.GetComponent<Gun>().minigunBulletsShot >= 200) {
                    shooting = false;
                    transform.Find("BulletSpawnPoint/Muzzle").GetComponent<SpriteRenderer>().enabled = false;
                    runningShootTime = 0f;
                    whatEnemyIsDoing = CurrentStance.Loading;
                    standardMovement = 115f;
                }
                
                break;
            case CurrentStance.Loading:
                runningShootTime += Time.deltaTime;

                shooting = false;
                walking = false;
                ai.canMove = false;
                ai.canSearch = false;

                if (runningShootTime >= 1.5f) {
                    gun.GetComponent<Gun>().FillMagazine();
                    runningShootTime = 0f;
                    doneOnce = false;
                    doneOnce2 = false;
                    whatEnemyIsDoing = CurrentStance.Waiting;
                }
                break;
        }
	}

    void Animations() {
        animator.SetBool("Walking", walking);
        animator.SetBool("Shooting", shooting);
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

    public void Die() {
        dead = true;
        //Corpse
        //RandomizeAndSpawnCorpse();
        //Blood
        Instantiate<GameObject>(dieBloodBigPrefab, transform.position, Quaternion.identity);
        Instantiate<GameObject>(dieBloodSmallPrefab, transform.position, Quaternion.identity);

        //Destroy targets and alerts(! and ?)
        Destroy(targetObject, 0f);
        Destroy(em);


        //Sound
        gameManager.GetComponent<SoundManager>().PlaySound("BloodSplatter", true);
        Destroy(gameObject, 0f);
    }

    public void DamageEnemy(float damageValue) {
        //blood
        Instantiate<GameObject>(dieBloodSmallPrefab, transform.position, Quaternion.identity);
        //sound

        enemyHealth -= damageValue;

        if (enemyHealth <= 0f && !dead) {
            Die();
        }

    }

    public float GetRunningTime() {
        return runningWaitTime;
    }
}
