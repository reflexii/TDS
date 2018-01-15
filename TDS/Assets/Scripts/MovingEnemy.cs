using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour {

    public enum CurrentStance { Moving, Shooting, SearchingPlayer, WaitingToMove};
    public CurrentStance whatEnemyIsDoing;
    public string enemyName = "Enemy1";
    public Transform[] movementPositionList;
    public float WaitTimeBeforeNextWaypoint = 2f;
    public GameObject targetPrefab;
    public GameObject wayPointPrefab;
    public Transform targetParentObject;
    public bool shoot = false;
    public float timeBeforeShooting;
    public GameManager gameManager;
    public GameObject gun;
    public Vector3 playerSearchPosition;
    public bool startSearching = false;
    public int turnIndex = 0;
    public AIPath ai;
    public float enemyHealth = 60f;
    public GameObject questionMarkPrefab;
    public GameObject exclamationMarkPrefab;
    public GameObject dieBloodBigPrefab;
    public GameObject dieBloodSmallPrefab;
    public bool destroyWaypoints = true;
    public bool backstabbable = false;

    //animation
    private int weaponUsed;
    private bool walking = false; 
    private Animator animator;

    private Transform wayPointParentObject;
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
    private GameObject qm;
    private GameObject em;
    private GameObject symbolObject;
    private bool dead = false;
    private bool doneOnce = false;
    private bool noticeSoundOnce = false;

    //corpses
    public GameObject greenCorpse1;
    public GameObject greenCorpse2;
    public GameObject greenCorpse3;
    public GameObject greenCorpse4;
    public GameObject redCorpse1;
    public GameObject redCorpse2;
    public GameObject redCorpse3;
    public GameObject redCorpse4;

    private GameObject player;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");

        if (GameObject.Find("WayPoints") == null) {
            GameObject g = new GameObject();
            g.name = "WayPoints";
        }
        wayPointParentObject = GameObject.Find("WayPoints").transform;
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

    void Start() {
        //Generate waypoint list
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
        target.name = enemyName + " target";
        ai.target = target.transform;
        targetObject = target;
    }

    void RandomizeAndSpawnCorpse() {
        int randomize = Random.Range(0, 4);
        GameObject chosenCorpse = null;
        if (gun.GetComponent<Gun>().weaponInUse == Gun.Weapons.Pistol || gun.GetComponent<Gun>().weaponInUse == Gun.Weapons.SMG) {
            switch(randomize) {
                case 0:
                    chosenCorpse = greenCorpse1;
                    break;
                case 1:
                    chosenCorpse = greenCorpse2;
                    break;
                case 2:
                    chosenCorpse = greenCorpse3;
                    break;
                case 3:
                    chosenCorpse = greenCorpse4;
                    break;
            }
        } else if (gun.GetComponent<Gun>().weaponInUse == Gun.Weapons.Shotgun || gun.GetComponent<Gun>().weaponInUse == Gun.Weapons.Rifle) {
            switch (randomize) {
                case 0:
                    chosenCorpse = redCorpse1;
                    break;
                case 1:
                    chosenCorpse = redCorpse2;
                    break;
                case 2:
                    chosenCorpse = redCorpse3;
                    break;
                case 3:
                    chosenCorpse = redCorpse4;
                    break;
            }
        }

        Instantiate<GameObject>(chosenCorpse, transform.position, Quaternion.identity);
    }

    void Animations() {
        animator.SetInteger("WeaponUsed", weaponUsed);
        animator.SetBool("Walking", walking);

        switch (transform.Find("Gun").GetComponent<Gun>().weaponInUse) {
            case Gun.Weapons.Pistol:
                weaponUsed = 1;
                break;
            case Gun.Weapons.Shotgun:
                weaponUsed = 2;
                break;
            case Gun.Weapons.SMG:
                weaponUsed = 3;
                break;
            case Gun.Weapons.Rifle:
                weaponUsed = 4;
                break;
        }
    }

    void DropGun() {
        gun.GetComponent<Collider2D>().enabled = true;

        Vector3 dir = (player.transform.position - transform.position).normalized;
        gun.GetComponent<Gun>().throwDirection = -dir;
        gun.GetComponent<Gun>().throwWeapon = true;
        //gun.transform.position += -dir * 0.3f;

        gun.transform.parent = null;
        gun.GetComponent<Gun>().gunOnTheFloor = true;
        gun.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gun = null;
    }

    void CreateAlerts()
    {
        if (GameObject.Find("Symbols") == null) {
            symbolObject = new GameObject();
            symbolObject.name = "Symbols";
        } else {
            symbolObject = GameObject.Find("Symbols");
        }
        qm = Instantiate<GameObject>(questionMarkPrefab, transform.position + new Vector3(0f, 0.9f, 0f), Quaternion.identity);
        qm.transform.parent = symbolObject.transform;
        qm.GetComponent<SpriteRenderer>().enabled = false;
        em = Instantiate<GameObject>(exclamationMarkPrefab, transform.position + new Vector3(0f, 0.9f, 0f), Quaternion.identity);
        em.transform.parent = symbolObject.transform;
        em.GetComponent<SpriteRenderer>().enabled = false;
    }

    void UpdateAlerts()
    {
        qm.transform.position = transform.position + new Vector3(0f, 0.9f, 0f);
        em.transform.position = transform.position + new Vector3(0f, 0.9f, 0f);
    }

    public void Die()
    {
        dead = true;
        //Corpse
        RandomizeAndSpawnCorpse();
        //Blood
        Instantiate<GameObject>(dieBloodBigPrefab, transform.position, Quaternion.identity);
        Instantiate<GameObject>(dieBloodSmallPrefab, transform.position, Quaternion.identity);
        
        if (movementPositionList.Length > 0 && destroyWaypoints) {
            for (int i = 0; i < movementPositionList.Length; i++) {
                Destroy(movementPositionList[i].gameObject, 0f);
            }
        }

        GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>().enemiesLeft--;
        

        //Destroy targets and alerts(! and ?)
        Destroy(targetObject, 0f);
        Destroy(em);
        Destroy(qm);
        DropGun();


        //Sound
        gameManager.GetComponent<SoundManager>().PlaySound("Blood2", true);
        Destroy(gameObject, 0f);
    }

    public void DamageEnemy(float damageValue)
    {
        //blood
        Instantiate<GameObject>(dieBloodSmallPrefab, transform.position, Quaternion.identity);
        //sound
        gameManager.GetComponent<SoundManager>().PlaySound("Blood1", true);

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

        if (enemyHealth <= 0f && !dead)
        {
            Die();
        }

    }
	
	
	void Update () {

        UpdateAlerts();
        Animations();
        
        switch (whatEnemyIsDoing)
        {
            case CurrentStance.Moving:
                walking = true;
                doneOnce = false;
                noticeSoundOnce = false;
                em.GetComponent<SpriteRenderer>().enabled = false;
                qm.GetComponent<SpriteRenderer>().enabled = false;
                turnIndex = 0;
                runningWaitTime = 0f;
                transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = false;
                startSearching = false;
                break;
            case CurrentStance.SearchingPlayer:
                doneOnce = false;
                noticeSoundOnce = false;
                transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = false;
                searchingTime += Time.deltaTime;
                runningWaitTime += Time.deltaTime;
                targetObject.transform.position = playerSearchPosition;
                shootingTime = 0f;
                

                if (searchingTime >= 0.4f)
                {
                    walking = true;
                    ai.canMove = true;
                    ai.canSearch = true;
                    em.GetComponent<SpriteRenderer>().enabled = false;
                    qm.GetComponent<SpriteRenderer>().enabled = true;
                } else {
                    walking = false;
                }

                if (startSearching) {
                    Search(5f);
                } else
                {
                    runningWaitTime = 0f;
                }
                break;
            case CurrentStance.Shooting:
                walking = false;
                startSearching = false;
                searchingTime = 0f;
                runningWaitTime = 0f;
                runningTurnTime = 0f;
                shootingTime += Time.deltaTime;

                if (shootingTime < 0.2f)
                {
                    em.GetComponent<SpriteRenderer>().enabled = false;
                    qm.GetComponent<SpriteRenderer>().enabled = true;

                    if (!noticeSoundOnce) {
                        //gameManager.GetComponent<SoundManager>().PlaySound("EnemyNotice", false);
                        noticeSoundOnce = true;
                    }
                }

                if (shootingTime >= 0.2f)
                {
                    ai.canMove = false;
                    ai.canSearch = false;
                    em.GetComponent<SpriteRenderer>().enabled = true;
                    qm.GetComponent<SpriteRenderer>().enabled = false;
                    noticeSoundOnce = false;
                }
                if (shootingTime >= (timeBeforeShooting / 1.3f))
                {
                    transform.up = player.transform.position - transform.position;

                }
                if (shootingTime >= timeBeforeShooting && gun.GetComponent<Gun>().currentMagazineSize > 0f)
                {
                    gun.GetComponent<Gun>().Shoot();
                }

                if (gun.GetComponent<Gun>().currentMagazineSize <= 0f) {
                    transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = false;
                }

                if (shootingTime >= timeBeforeShooting && gun.GetComponent<Gun>().currentMagazineSize <= 0f && !doneOnce) {
                    gameManager.GetComponent<SoundManager>().PlaySound("EmptyGun", true);
                    doneOnce = true;
                }
                break;
            case CurrentStance.WaitingToMove:
                transform.Find("BulletSpawnPoint").transform.Find("Muzzle").GetComponent<SpriteRenderer>().enabled = false;
                noticeSoundOnce = false;
                doneOnce = false;
                walking = false;
                startSearching = false;
                em.GetComponent<SpriteRenderer>().enabled = false;
                qm.GetComponent<SpriteRenderer>().enabled = false;
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
        walking = false;

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
    public float GetRunningTime()
    {
        return runningWaitTime;
    }
}
