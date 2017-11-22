using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIP : MonoBehaviour {

    public enum CurrentStance { Stand, Follow };
    public CurrentStance whatVIPIsDoing;
    public AIPath ai;
    public Transform targetParentObject;
    public GameObject targetPrefab;
    public GameObject dieBloodBigPrefab;
    public GameObject dieBloodSmallPrefab;
    public bool following = false;

    private GameObject targetObject;
    private Animator animator;
    private GameObject player;
    private bool walking = false;
    private GameObject damageCollider;
    public bool doneOnce = false;

    void Awake () {
        ai = GetComponent<AIPath>();
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        targetParentObject = GameObject.Find("EnemyTargets").transform;
        damageCollider = transform.Find("damageCollider").gameObject;

        if (!following) {
            whatVIPIsDoing = CurrentStance.Stand;
        } else {
            whatVIPIsDoing = CurrentStance.Follow;
        }

	}

    private void Start() {
        GameObject target = Instantiate<GameObject>(targetPrefab, player.transform.position, Quaternion.identity);

        target.GetComponent<TargetMovement>().enemyObject = gameObject;
        target.GetComponent<TargetMovement>().scientist = false;
        target.GetComponent<TargetMovement>().movingEnemy = false;
        target.name = "VIP Target";
        ai.target = target.transform;
        targetObject = target;
    }

    void Animations() {
        animator.SetBool("Walking", walking);
    }
	
	void Update () {

        if (!doneOnce && following) {
            whatVIPIsDoing = CurrentStance.Follow;
            doneOnce = true;
        }

        Animations();
        targetObject.transform.position = player.transform.position;

		switch(whatVIPIsDoing) {
            case CurrentStance.Stand:
                walking = false;
                ai.canMove = false;
                break;
            case CurrentStance.Follow:
                walking = true;
                ai.canMove = true;
                break;
        }
	}
}
