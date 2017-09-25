using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour {

    public GameObject enemyObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == enemyObject &&
            enemyObject.GetComponent<MovingEnemy>().whatEnemyIsDoing != MovingEnemy.CurrentStance.SearchingPlayer &&
            enemyObject.GetComponent<MovingEnemy>().whatEnemyIsDoing != MovingEnemy.CurrentStance.Shooting)
        {
            enemyObject.GetComponent<MovingEnemy>().whatEnemyIsDoing = MovingEnemy.CurrentStance.WaitingToMove;
        } else if (collision.gameObject == enemyObject && enemyObject.GetComponent<MovingEnemy>().whatEnemyIsDoing == MovingEnemy.CurrentStance.SearchingPlayer) {
            enemyObject.GetComponent<MovingEnemy>().startSearching = true;
        }
    }
}
