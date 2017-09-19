using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour {

    public enum MovementType { ReturnToStartAfterFinish, FollowPathBackToStart};
    public MovementType move;
    public string enemyName = "Enemy1";
    public Transform[] movementPositionList;
    public float WaitTimeBeforeNextWaypoint = 2f;
    public GameObject targetPrefab;
    public GameObject wayPointPrefab;
    public Transform wayPointParentObject;
    public Transform targetParentObject;
    public bool waitToMove = false;

    private AIPath ai;
    private float runningWaitTime = 0f;
    private int positionIndex = 1;
    private GameObject targetObject;
    private int listLength;
    

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
        if (waitToMove)
        {
            runningWaitTime += Time.deltaTime;
        }

        Move();
        
	}

    void Move()
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
            waitToMove = false;
        }
    }
}
