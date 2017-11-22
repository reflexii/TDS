using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour {

    public GameObject enemyObject;
    public bool movingEnemy = true;
    public bool scientist = false;

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
        }
    }
}
