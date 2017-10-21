using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public float rotateAngle = 45f;
    public float rotateTime = 1f;
    public bool rightFirst = true;

    private float rotatedAmount;
    private Vector3 dir;
    private SecurityCam laser;

    //return to original rotation
    private float rotateSpeed;
    private bool returning = false;
    private bool doneOnce = false;
    private bool doneOnce2 = false;
    private Quaternion savedRotation;
    private float savedEuler;
    private Quaternion startingRotate;
    private float startEuler;
    private float rate;
    private float runningOriginalRotate = 0f;

	void Start () {

        laser = transform.Find("Laser").GetComponent<SecurityCam>();
        rotateSpeed = rotateAngle / rotateTime;

		if (rightFirst)
        {
            dir = Vector3.back;
        } else
        {
            dir = Vector3.forward;
        }
	}
	
	void Update () {

        if (!laser.playerSpotted && !returning)
        {
            doneOnce = false;
            doneOnce2 = false;
            transform.Rotate(dir, rotateSpeed * Time.deltaTime);
            rotatedAmount += rotateSpeed * Time.deltaTime;

            if (rotatedAmount >= rotateAngle)
            {
                if (dir == Vector3.back)
                {
                    dir = Vector3.forward;
                }
                else
                {
                    dir = Vector3.back;
                }

                rotatedAmount = 0f;
            }
        } else if (laser.playerSpotted)
        {
            returning = true;
            if (!doneOnce)
            {
                savedRotation = transform.rotation;
                doneOnce = true;
            }

            Vector3 direction = laser.playerObject.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        } else if (!laser.playerSpotted && returning)
        {
            if (!doneOnce2)
            {
                startingRotate = transform.rotation;

                savedEuler = savedRotation.eulerAngles.z;
                startEuler = startingRotate.eulerAngles.z;
                float absoluteRotation = Mathf.Abs(startEuler - savedEuler);
                if (absoluteRotation >= 180f) {
                    absoluteRotation -= 360f;
                    absoluteRotation = Mathf.Abs(absoluteRotation);
                }
                rate = absoluteRotation / rotateSpeed;
                Debug.Log("Absolute: " + absoluteRotation + " TurnRatePerSecond: " + rotateSpeed + " Rate: " + rate);

                doneOnce2 = true;
            }
            ReturnToOriginalRotation();
        }
        
    }

    void ReturnToOriginalRotation()
    {
        runningOriginalRotate += Time.deltaTime / rate;

        transform.rotation = Quaternion.Lerp(startingRotate, savedRotation, runningOriginalRotate);
        if (runningOriginalRotate >= 1f)
        {
            returning = false;
            runningOriginalRotate = 0f;
        }
    }
}
