using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmooth : MonoBehaviour {

    public Transform objectToLookAt;
    public bool smooth = true;
    public bool smoothAimZoom = true;
    public float smoothSpeed = 0.15f;
    public GameObject player;
    public GameManager gameManager;

    private GameObject boundsBox;
    private Vector3 minBounds;
    private Vector3 maxBounds;
    private float halfHeight;
    private float halfWidth;
    private Camera mainCamera;

    private void Start() {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        boundsBox = GameObject.Find("Bounds");

        //keep camera inside the map only if possible
        minBounds = boundsBox.GetComponent<Collider2D>().bounds.min;
        maxBounds = boundsBox.GetComponent<Collider2D>().bounds.max;

        halfHeight = GetComponent<Camera>().orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;

    }

    private Vector3 offset = new Vector3(0f, 0f, -10f);

	private void LateUpdate () {
        Vector3 desiredPosition = objectToLookAt.transform.position + offset;

        if (smooth) {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        } else if (smoothAimZoom) {
            Vector3 mouseScreenPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            transform.position = Vector3.Lerp(objectToLookAt.position, mouseScreenPosition + offset, 0.25f);
        }
        else {
            transform.position = desiredPosition;
        }

        //keep camera inside the map only if possible
        float clampedX = Mathf.Clamp(transform.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float clampedY = Mathf.Clamp(transform.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

	}
}
