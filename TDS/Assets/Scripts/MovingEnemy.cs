using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour {

    public enum MovementType { ReturnToStartAfterFinish, FollowPathBackToStart};
    public enum CurrentStance { Moving, Shooting, SearchingPlayer, WaitingToMove};
    public MovementType move;
    public CurrentStance whatEnemyIsDoing;
    public string enemyName = "Enemy1";
    public Transform[] movementPositionList;
    public float WaitTimeBeforeNextWaypoint = 2f;
    public GameObject targetPrefab;
    public GameObject wayPointPrefab;
    public Transform wayPointParentObject;
    public Transform targetParentObject;
    public bool shoot = false;
    public float timeBeforeShooting;
    public GameManager gameManager;
    public GameObject gun;
    public Vector3 playerSearchPosition;
    public bool startSearching = false;
    public int turnIndex = 0;
    public AIPath ai;

    private float runningWaitTime = 0f;
    private float runningTurnTime = 3f;
    private int positionIndex = 1;
    private GameObject targetObject;
    private int listLength;
    private float shootingTime = 0f;
    private float searchingTime = 0f;
    private int startingDirection = 1;
    private float waitRotateSpeed = 4f;
    private float searchRotateSpeed = 6f;
    private bool didOnce = false;
    private float randomize = 0f;
    


    private void Awake()
    {
        ai = GetComponent<AIPath>();
    }

    void Start()
    {
        listLength = movementPositionList.Length;
        GameObject wp = Instantiate<GameObject>(wayPointPrefab, transform.position, Quaternion.identity, wayPointParentObject);
        wp.name = "Waypoint";
        movementPositionList[0] = wp.transform;

        if (movementPositionList[1] == null)
        {
            Debug.Log("Error: You have to have waypoints!");
        }
        else
        {
            GameObject target = Instantiate<GameObject>(targetPrefab, movementPositionList[1].transform.position, Quaternion.identity, targetParentObject);
            target.GetComponent<TargetMovement>().enemyObject = gameObject;
            target.name = enemyName + " target";
            ai.target = target.transform;
            targetObject = target;
        }
    }
	
	
	void Update () {

        switch (whatEnemyIsDoing)
        {
            case CurrentStance.Moving:
                turnIndex = 0;
                runningWaitTime = 0f;
                break;
            case CurrentStance.SearchingPlayer:
                searchingTime += Time.deltaTime;
                runningWaitTime += Time.deltaTime;
                targetObject.transform.position = playerSearchPosition;
                shootingTime = 0f;
                

                if (searchingTime >= 0.4f)
                {
                    ai.canMove = true;
                    ai.canSearch = true;
                }

                if (startSearching) {
                    Search(5f);
                } else
                {
                    runningWaitTime = 0f;
                }
                break;
            case CurrentStance.Shooting:
                searchingTime = 0f;
                runningWaitTime = 0f;
                runningTurnTime = 0f;
                shootingTime += Time.deltaTime;

                if (shootingTime >= 0.2f)
                {
                    ai.canMove = false;
                    ai.canSearch = false;
                }
                if (shootingTime >= (timeBeforeShooting / 1.3f))
                {
                    transform.up = gameManager.Player.transform.position - transform.position;

                }
                if (shootingTime >= timeBeforeShooting)
                {
                    gun.GetComponent<Gun>().Shoot();
                }
                break;
            case CurrentStance.WaitingToMove:
                runningWaitTime += Time.deltaTime;
                runningTurnTime += Time.deltaTime;
                MoveTarget();

                if (!didOnce)
                {
                    randomize = Random.Range(0.75f, 1.25f);
                    didOnce = true;
                }

                if (runningTurnTime <= 2.5f && runningTurnTime >= randomize)
                {

                    waitRotateSpeed -= (Time.deltaTime * 7f);
                    if (waitRotateSpeed <= 0f)
                    {
                        waitRotateSpeed = 0f;
                    }

                    transform.Rotate(new Vector3(0f, 0f, 1f) * Time.deltaTime * (75f * startingDirection * waitRotateSpeed));

                    
                } else if (runningTurnTime > 2.5f)
                {
                    runningTurnTime = 0f;
                    waitRotateSpeed = 4f;
                    didOnce = false;
                    if (turnIndex % 4 == 1 || turnIndex % 4 == 2)
                    {
                        startingDirection = -1;
                    } else
                    {
                        startingDirection = 1;
                    }

                    turnIndex++;
                }
                break;
        }
        
	}

    void MoveTarget()
    {
        if (runningWaitTime >= WaitTimeBeforeNextWaypoint)
        {
            positionIndex++;
            if (positionIndex <= listLength-1)
            {
                targetObject.transform.position = movementPositionList[positionIndex].position;
            } else
            {
                positionIndex = 0;
                targetObject.transform.position = movementPositionList[positionIndex].position;
            }

            runningWaitTime = 0f;
            runningTurnTime = 0f;
            startSearching = false;
            whatEnemyIsDoing = CurrentStance.Moving;
        }
    }

    public void Search(float searchTime)
    {
        runningTurnTime += Time.deltaTime;

        if (runningWaitTime >= searchTime)
        {
            MoveTarget();
            ai.canMove = true;
            ai.canSearch = true;
        }

        if (!didOnce)
        {
            randomize = Random.Range(0.75f, 1.25f);
            didOnce = true;
        }

        if (runningTurnTime <= 2.5f && runningTurnTime >= randomize)
        {

            searchRotateSpeed -= (Time.deltaTime * 8f);
            if (searchRotateSpeed <= 0f)
            {
                searchRotateSpeed = 0f;
            }

            transform.Rotate(new Vector3(0f, 0f, 1f) * Time.deltaTime * (120f * startingDirection * searchRotateSpeed));


        }
        else if (runningTurnTime > 2.5f)
        {
            runningTurnTime = 0f;
            searchRotateSpeed = 6f;
            didOnce = false;
            if (turnIndex % 4 == 1 || turnIndex % 4 == 2)
            {
                startingDirection = -1;
            }
            else
            {
                startingDirection = 1;
            }

            turnIndex++;

        }
    }
}
