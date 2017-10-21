using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpotter : MonoBehaviour {

    public GameObject enemy;

    private GameManager gameManager;
    private bool isPlayerVisible = false;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1") && !IsPlayerVisible()) {
            isPlayerVisible = false;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1") && IsPlayerVisible()) {
            isPlayerVisible = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1")) {
            isPlayerVisible = false;
        }
        
    }

    private void Update() {
        if (isPlayerVisible) {
            enemy.GetComponent<MovingEnemy>().whatEnemyIsDoing = MovingEnemy.CurrentStance.Shooting;
            
        } else {
            if (enemy.GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.Shooting)
            {
                enemy.GetComponent<MovingEnemy>().whatEnemyIsDoing = MovingEnemy.CurrentStance.SearchingPlayer;
                enemy.GetComponent<MovingEnemy>().playerSearchPosition = gameManager.Player.transform.position;
            }        
        }
        
    }

    public bool IsPlayerVisible() {
        if (Physics2D.Raycast(enemy.transform.position, gameManager.Player.transform.position - enemy.transform.position, Vector3.Distance(gameManager.Player.transform.position, enemy.transform.position), 1 << LayerMask.NameToLayer("Wall"))) {
            return false;
        } else {
            return true;
        }
    }

    /*
    private void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(enemy.transform.position, gameManager.Player.transform.position - enemy.transform.position);
    }
    */
    
}
