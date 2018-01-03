using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

    public enum CurrentStance { Shooting, Waiting };
    public CurrentStance whatEnemyIsDoing;
    public GameManager gameManager;
    public GameObject gun;
    public AIPath ai;
    public Transform targetParentObject;
    public GameObject exclamationMarkPrefab;
    public GameObject targetPrefab;
    public Vector3 savedPlayerPosition;
    public Vector3 newPlayerPosition;

    private GameObject player;
    private GameObject symbolObject;
    private Animator animator;
    private GameObject em;
    private GameObject targetObject;
    private bool doneOnce = false;
    private bool doneOnce2 = false;
    private float runningShootTime = 0.0f;
    private Vector3 direction;
    private float distanceBetweenPositions;

    void Awake () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        gun = transform.Find("Gun").gameObject;
        ai = GetComponent<AIPath>();
        gun.GetComponent<Gun>().playerOwned = false;

        if (GameObject.Find("EnemyTargets") == null) {
            GameObject g = new GameObject();
            g.name = "EnemyTargets";
        }

        targetParentObject = GameObject.Find("EnemyTargets").transform;
        animator = GetComponent<Animator>();

        CreateAlerts();

    }

    private void Start() {
        /*
        GameObject target = Instantiate<GameObject>(targetPrefab, transform.position, Quaternion.identity, targetParentObject);

        target.GetComponent<TargetMovement>().enemyObject = gameObject;
        target.name = "Boss target";
        ai.target = target.transform;
        targetObject = target;
        */
    }

    void Update () {
        Animations();
        UpdateAlerts();

        switch (whatEnemyIsDoing) {
            case CurrentStance.Waiting:
                doneOnce = false;
                doneOnce2 = false;
                break;
            case CurrentStance.Shooting:

                runningShootTime += Time.deltaTime;

                //shoot 20 bullets towards player's old position, start rotating to the side the player was moving

                if (runningShootTime < 0.5f) {
                    if (!doneOnce) {
                        transform.up = player.transform.position - transform.position;
                        savedPlayerPosition = player.transform.position;
                        doneOnce = true;
                    }
                }
                //shoot 20 bullets towards saved player position
                if (runningShootTime >= 0.5f && gun.GetComponent<Gun>().minigunBulletsShot <= 10 && gun.GetComponent<Gun>().runningCooldown > gun.GetComponent<Gun>().minigunCooldown) {
                    gun.GetComponent<Gun>().BossShoot(savedPlayerPosition - transform.position);
                } else if (gun.GetComponent<Gun>().minigunBulletsShot > 10 && gun.GetComponent<Gun>().minigunBulletsShot < 70 && gun.GetComponent<Gun>().runningCooldown > gun.GetComponent<Gun>().minigunCooldown) {
                    if (!doneOnce2) {
                        newPlayerPosition = player.transform.position;
                        doneOnce2 = true;
                        direction = (newPlayerPosition - savedPlayerPosition).normalized;
                        distanceBetweenPositions = Vector3.Distance(newPlayerPosition, savedPlayerPosition);
                    }

                    savedPlayerPosition += (direction / 60f * distanceBetweenPositions*2f);
                    transform.up = savedPlayerPosition - transform.position;
                    gun.GetComponent<Gun>().BossShoot(savedPlayerPosition - transform.position);
                }
                
                break;
        }
	}

    void Animations() {

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

    public void Die() {
        //Boss dies
    }

    public void DamageEnemy(float damageValue) {
        //Boss take damage(from explosions etc only, no bullets)
    }
}
