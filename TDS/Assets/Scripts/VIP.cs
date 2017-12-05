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
    public float vipHealth = 200f;
    public float vipCurrentHealth;
    public GameObject corpse1;
    public GameObject corpse2;
    public GameObject exclamationMarkPrefab;

    private GameObject targetObject;
    private Animator animator;
    private GameObject player;
    private bool walking = false;
    private GameObject damageCollider;
    public bool doneOnce = false;
    private bool dead = false;
    private GameObject em;
    private GameObject symbolObject;

    void Awake () {
        ai = GetComponent<AIPath>();
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        targetParentObject = GameObject.Find("EnemyTargets").transform;
        damageCollider = transform.Find("damageCollider").gameObject;
        vipCurrentHealth = vipHealth;

        if (!following) {
            whatVIPIsDoing = CurrentStance.Stand;
        } else {
            whatVIPIsDoing = CurrentStance.Follow;
        }

        CreateAlerts();
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

    void Update () {

        if (!doneOnce && following) {
            whatVIPIsDoing = CurrentStance.Follow;
            doneOnce = true;
        }

        UpdateAlerts();
        Animations();
        targetObject.transform.position = player.transform.position;

		switch(whatVIPIsDoing) {
            case CurrentStance.Stand:
                walking = false;
                ai.canMove = false;
                em.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case CurrentStance.Follow:
                walking = true;
                ai.canMove = true;
                em.GetComponent<SpriteRenderer>().enabled = true;
                break;
        }
	}

    public void TakeDamage(float damageAmount) {
        //blood
        Instantiate<GameObject>(dieBloodSmallPrefab, transform.position, Quaternion.identity);
        //sound

        vipCurrentHealth -= damageAmount;

        if (vipCurrentHealth <= 0f && !dead) {
            Die();
        }
    }

    public void Die() {
        dead = true;

        RandomizeAndSpawnCorpse();

        Instantiate<GameObject>(dieBloodBigPrefab, transform.position, Quaternion.identity);
        Instantiate<GameObject>(dieBloodSmallPrefab, transform.position, Quaternion.identity);

        GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().MissionFailed("VIP has died.");

        Destroy(em);
        Destroy(targetObject, 0f);
        Destroy(gameObject, 0f);
    }

    public void RandomizeAndSpawnCorpse() {
        int randomize = Random.Range(0, 2);
        GameObject chosenCorpse;

        if (randomize == 0) {
            chosenCorpse = corpse1;
        } else {
            chosenCorpse = corpse2;
        }

        Instantiate<GameObject>(chosenCorpse, transform.position, Quaternion.identity);
    }
}
