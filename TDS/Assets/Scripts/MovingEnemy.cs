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

    private AIPath ai;
    private float runningWaitTime = 0f;
    private float runningTurnTime = 0f;
    private int turnIndex = 0;
    private int positionIndex = 1;
    private GameObject targetObject;
    private int listLength;
    private float shootingTime = 0f;
    private float searchingTime = 0f;


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
                break;
            case CurrentStance.SearchingPlayer:
                searchingTime += Time.deltaTime;
                targetObject.transform.position = playerSearchPosition;
                shootingTime = 0f;

                if (searchingTime >= 0.4f)
                {
                    ai.canMove = true;
                    ai.canSearch = true;
                }
                break;
            case CurrentStance.Shooting:
                searchingTime = 0f;
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
                float random;
                int startingDirection;
                int randomDirection = Random.Range(0, 2);

                if (randomDirection == 0)
                {
                    startingDirection = 1;
                } else
                {
                    startingDirection = -1;
                }

                if (turnIndex == 0)
                {
                    random = Random.Range(80f, 90f) * startingDirection;
                } else
                {
                    random = Random.Range(160f, 180f) * startingDirection;
                }

                if (runningTurnTime <= 1f)
                {
                    if (turnIndex == 0)
                    {
                        transform.Rotate(new Vector3(0f, 0f, 1f) * Time.deltaTime * (random / 1f));
                    } else
                    {
                        transform.Rotate(new Vector3(0f, 0f, 1f) * Time.deltaTime * (random / 2f));
                    }
                    
                } else
                {
                    runningTurnTime = 0f;
                    startingDirection *= -1;
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
            whatEnemyIsDoing = CurrentStance.Moving;
        }
    }
}
