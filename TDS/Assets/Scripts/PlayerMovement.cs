using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    //MouseLook
    public Camera main;


    public float movementSpeed;
    public bool drawPlayerGizmos = true;

    //Raycasting and movement
    private bool disableTop = false;
    private bool disableBottom = false;
    private bool disableLeft = false;
    private bool disableRight = false;

    void Update () {
        Movement();
        MouseLook();
        WallCheck();
	}

    void Movement()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !disableTop && !disableRight) {
            transform.position += new Vector3(1f, 1f).normalized * movementSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !disableTop && !disableLeft) {
            transform.position += new Vector3(-1f, 1f).normalized * movementSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !disableBottom && !disableRight) {
            transform.position += new Vector3(1f, -1f).normalized * movementSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && !disableBottom && !disableLeft) {
            transform.position += new Vector3(-1f, -1f).normalized * movementSpeed * Time.deltaTime;
        } else {
            if (Input.GetKey(KeyCode.W) && !disableTop) {
                transform.position += new Vector3(0f, 1f) * movementSpeed * Time.deltaTime;
            }
             else if (Input.GetKey(KeyCode.S) && !disableBottom) {
                transform.position += new Vector3(0f, -1f) * movementSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.A) && !disableLeft) {
                transform.position += new Vector3(-1f, 0f) * movementSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D) && !disableRight) {
                transform.position += new Vector3(1f, 0f) * movementSpeed * Time.deltaTime;
            }
        }
    }

    void MouseLook() {
        // convert mouse position into world coordinates
        Vector2 mouseScreenPosition = main.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        Vector2 direction = (mouseScreenPosition - (Vector2)transform.position).normalized;

        // set vector of transform directly
        transform.right = direction;
    }

    void WallCheck() {
        if (Physics2D.Raycast(transform.position, Vector3.up, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
            disableTop = true;
        } else {
            disableTop = false;
        }
        if (Physics2D.Raycast(transform.position, Vector3.down, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
            disableBottom = true;
        } else {
            disableBottom = false;
        }
        if (Physics2D.Raycast(transform.position, Vector3.left, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
            disableLeft = true;
        } else {
            disableLeft = false;
        }
        if (Physics2D.Raycast(transform.position, Vector3.right, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
            disableRight = true;
        } else {
            disableRight = false;
        }
    }

    private void OnDrawGizmos() {
        if (drawPlayerGizmos) {
            Gizmos.color = Color.green;

            //Top
            Gizmos.DrawRay(transform.position, Vector3.up);
            //Bottom
            Gizmos.DrawRay(transform.position, Vector3.down);
            //Left
            Gizmos.DrawRay(transform.position, Vector3.left);
            //Right
            Gizmos.DrawRay(transform.position, Vector3.right);
        }
    }
}
