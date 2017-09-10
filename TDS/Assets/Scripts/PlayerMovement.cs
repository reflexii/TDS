using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private GameManager gameManager;
    public float movementSpeed;
    public bool drawPlayerGizmos = true;

    //Raycasting and movement
    private bool disableTop = false;
    private bool disableBottom = false;
    private bool disableLeft = false;
    private bool disableRight = false;

    //Gun
    public Gun gun;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update () {
        Movement();
        MouseLook();
        WallCheck();
        Shooting();
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
        Vector2 mouseScreenPosition = gameManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        Vector2 direction = (mouseScreenPosition - (Vector2)transform.position).normalized;

        // set vector of transform directly
        transform.right = direction;
    }

    void WallCheck() {
        if (Input.GetKey(KeyCode.W)) {
            if (Physics2D.Raycast(transform.position, Vector3.up, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
                disableTop = true;
            } else {
                disableTop = false;
            }
        }
        if (Input.GetKey(KeyCode.S)) {
            if (Physics2D.Raycast(transform.position, Vector3.down, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
                disableBottom = true;
            } else {
                disableBottom = false;
            }
        }
        if (Input.GetKey(KeyCode.A)) {
            if (Physics2D.Raycast(transform.position, Vector3.left, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
                disableLeft = true;
            } else {
                disableLeft = false;
            }
        }    
        if (Input.GetKey(KeyCode.D)) {
            if (Physics2D.Raycast(transform.position, Vector3.right, 1f, 1 << LayerMask.NameToLayer("Wall"))) {
                disableRight = true;
            } else {
                disableRight = false;
            }
        }
        
    }

    void Shooting() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            gun.Shoot();
        }
    }

    private void OnDrawGizmos() {
        if (drawPlayerGizmos) {
            Gizmos.color = Color.green;

            //Top
            if (Input.GetKey(KeyCode.W)) {
                Gizmos.DrawRay(transform.position, Vector3.up);
            }
            
            //Bottom
            if (Input.GetKey(KeyCode.S)) {
                Gizmos.DrawRay(transform.position, Vector3.down);
            }
            //Left
            if (Input.GetKey(KeyCode.A)) {
                Gizmos.DrawRay(transform.position, Vector3.left);
            }
            //Right
            if (Input.GetKey(KeyCode.D)) {
                Gizmos.DrawRay(transform.position, Vector3.right);
            }
            
            
            
        }
    }
}
