using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmooth : MonoBehaviour {

    public Transform objectToLookAt;
    public bool smooth = true;
    public bool smoothAimZoom = true;
    public bool smoothAimZoomSpecial = true;
    public float smoothSpeed = 0.15f;
    private Camera main;
    private GameObject player;
    private float defaultOrthographicSize;

    private void Awake() {
        main = GetComponent<Camera>();
        player = GameObject.Find("Player");
        defaultOrthographicSize = main.orthographicSize;
    }

    private Vector3 offset = new Vector3(0f, 0f, -10f);

	private void LateUpdate () {
        Vector3 desiredPosition = objectToLookAt.transform.position + offset;

        if (smooth) {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        } else if (smoothAimZoom) {
            Vector3 mouseScreenPosition = main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = Vector3.Lerp(objectToLookAt.position, mouseScreenPosition + offset, 0.25f);
        } else if (smoothAimZoomSpecial) {
            Vector3 mouseScreenPosition = main.ScreenToWorldPoint(Input.mousePosition);
            float distance = Vector3.Distance(mouseScreenPosition, player.transform.position);

            transform.position = Vector3.Lerp(objectToLookAt.position, mouseScreenPosition + offset, 0.1f);
            Debug.Log(distance);
            if (distance <= 8f) {
                main.orthographicSize = 12f;
            } else {
                main.orthographicSize = defaultOrthographicSize + ((distance-8f) / 10f);
                if (main.orthographicSize > 15.0f) {
                    main.orthographicSize = 15.0f;
                }
            }
            
        }
        else {
            transform.position = desiredPosition;
        }
	}
}
