using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneEnemy : MonoBehaviour {

    public float movementSpeed = 5f;
    public bool horizontal = true;
    public bool walking = true;

    private Animator animator;

	void Start () {
        animator = GetComponent<Animator>();
	}
	
	void Update () {

        Animations();

        if (walking) {
            if (horizontal) {
                gameObject.transform.position += new Vector3(1f, 0f, 0f) * movementSpeed * Time.deltaTime;
            } else {
                gameObject.transform.position += new Vector3(0f, -1f, 0f) * movementSpeed * Time.deltaTime;
            }
        }
        
	}

    void Animations() {
        animator.SetBool("Walking", walking);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyWalk")) {
            walking = false;
        }
    }
}
