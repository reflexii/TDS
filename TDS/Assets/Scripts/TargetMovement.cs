using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour {

    public GameObject enemyObject;
    public bool movingEnemy = true;
    public bool scientist = false;
    public bool boss = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (movingEnemy) {
            if (collision.gameObject == enemyObject &&
            enemyObject.GetComponent<MovingEnemy>().whatEnemyIsDoing != MovingEnemy.CurrentStance.SearchingPlayer &&
            enemyObject.GetComponent<MovingEnemy>().whatEnemyIsDoing != MovingEnemy.CurrentStance.Shooting) {
                enemyObject.GetComponent<MovingEnemy>().whatEnemyIsDoing = MovingEnemy.CurrentStance.WaitingToMove;

            } else if (collision.gameObject == enemyObject && enemyObject.GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.SearchingPlayer) {
                enemyObject.GetComponent<MovingEnemy>().startSearching = true;
            }
        } else if (scientist) {
            if (collision.gameObject == enemyObject && 
            enemyObject.GetComponent<Scientist>().whatEnemyIsDoing != Scientist.CurrentStance.Alerted &&
            enemyObject.GetComponent<Scientist>().whatEnemyIsDoing != Scientist.CurrentStance.Hiding) {
                enemyObject.GetComponent<Scientist>().whatEnemyIsDoing = Scientist.CurrentStance.WaitingToMove;
            }
        } else if (!movingEnemy && !scientist && !boss) {
            if (collision.gameObject == enemyObject && enemyObject.GetComponent<VIP>().whatVIPIsDoing == VIP.CurrentStance.Follow && enemyObject.GetComponent<VIP>().following) {
                collision.gameObject.GetComponent<VIP>().whatVIPIsDoing = VIP.CurrentStance.Stand;
            }
        } else if (boss) {
            if (collision.gameObject == enemyObject && enemyObject.GetComponent<Boss>().whatEnemyIsDoing != Boss.CurrentStance.Shooting && enemyObject.GetComponent<Boss>().whatEnemyIsDoing != Boss.CurrentStance.Loading) {
                collision.gameObject.GetComponent<Boss>().whatEnemyIsDoing = Boss.CurrentStance.Waiting;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!movingEnemy && !scientist && !boss) {
            if (collision.gameObject == enemyObject && enemyObject.GetComponent<VIP>().whatVIPIsDoing == VIP.CurrentStance.Stand && enemyObject.GetComponent<VIP>().following) {
                collision.gameObject.GetComponent<VIP>().whatVIPIsDoing = VIP.CurrentStance.Follow;
                enemyObject.GetComponent<VIP>().doneOnce = false;
            }
        }
    }
}
