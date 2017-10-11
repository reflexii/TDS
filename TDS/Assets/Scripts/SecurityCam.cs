using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCam : MonoBehaviour {

    public float laserWidth = 0.5f;
    public float laserMaxLength = 5f;

    private LineRenderer laser;
    
	void Start () {
        laser = GetComponent<LineRenderer>();
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laser.SetPositions(initLaserPositions);
        laser.startWidth = laserWidth;
        laser.endWidth = laserWidth;
	}
	
	void Update () {
        ShootLaser(transform.position, transform.rotation * Vector3.right);
	}

    void ShootLaser(Vector3 targetPos, Vector3 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(targetPos, dir, laserMaxLength, 1 << LayerMask.NameToLayer("Wall"));
        Vector3 endPos = targetPos + (laserMaxLength * dir);

        if (hit.collider != null)
        {
            endPos = hit.point;
            Debug.Log("Hit wall!");
        }

        laser.SetPosition(0, targetPos);
        laser.SetPosition(1, endPos);
        Debug.Log("Shooting laser: " + targetPos + " - " + endPos);
    }
}
