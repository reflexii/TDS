using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmooth : MonoBehaviour {

    public Transform objectToLookAt;
    public bool smooth = true;
    public bool smoothAimZoom = true;
    public bool smoothAimZoomSpecial = true;
    public float smoothSpeed = 0.15f;
    public GameObject player;
    public GameManager gameManager;
    private float defaultOrthographicSize;

    private void Awake() {
        defaultOrthographicSize = gameManager.mainCamera.orthographicSize;
    }

    private Vector3 offset = new Vector3(0f, 0f, -10f);

	private void LateUpdate () {
        Vector3 desiredPosition = objectToLookAt.transform.position + offset;

        if (smooth) {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        } else if (smoothAimZoom) {
            Vector3 mouseScreenPosition = gameManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);

            transform.position = Vector3.Lerp(objectToLookAt.position, mouseScreenPosition + offset, 0.25f);
        } else if (smoothAimZoomSpecial) {
            Vector3 mouseScreenPosition = gameManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            float distance = Vector3.Distance(mouseScreenPosition, player.transform.position);

            transform.position = Vector3.Lerp(objectToLookAt.position, mouseScreenPosition + offset, 0.1f);
            if (distance <= 8f) {
                gameManager.mainCamera.orthographicSize = 12f;
            } else {
                gameManager.mainCamera.orthographicSize = defaultOrthographicSize + ((distance-8f) / 10f);
                if (gameManager.mainCamera.orthographicSize > 15.0f) {
                    gameManager.mainCamera.orthographicSize = 15.0f;
                }
            }
            
        }
        else {
            transform.position = desiredPosition;
        }
	}
}
