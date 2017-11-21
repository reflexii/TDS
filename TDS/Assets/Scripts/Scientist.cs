using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist : MonoBehaviour {

    public enum CurrentStance { Moving, Alerted, Hiding};
    public CurrentStance whatEnemyIsDoing;
    public Transform[] movementPositionList;
    public GameObject wayPointPrefab;
    public GameObject targetPrefab;
    public Transform targetParentObject;

    private int listLength;
    private Transform wayPointParentObject;

    void Awake () {
		
	}
	
    void Start() {
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
    }

	void Update () {
		
	}
}
