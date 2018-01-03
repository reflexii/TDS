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

    private GameObject player;
    private GameObject symbolObject;
    private Animator animator;
    private GameObject em;

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
	
	void Update () {
        Animations();
        UpdateAlerts();

        switch (whatEnemyIsDoing) {
            case CurrentStance.Waiting:
                break;
            case CurrentStance.Shooting:
                transform.up = player.transform.position - transform.position;
                gun.GetComponent<Gun>().Shoot();
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
