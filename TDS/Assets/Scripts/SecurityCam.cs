using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCam : MonoBehaviour {

    public float laserWidth = 0.5f;
    public float laserMaxLength = 5f;
    public Material greenMaterial;
    public Material redMaterial;
    public Material yellowMaterial;
    public bool playerSpotted = false;
    [HideInInspector]
    public GameObject playerObject;

    private LineRenderer laser;
    private GameObject laserStart;

    void Start () {
        laser = GetComponent<LineRenderer>();
        laserStart = transform.parent.Find("LaserStart").gameObject;
        Vector3[] initLaserPositions = new Vector3[2] { transform.position, transform.position };
        laser.SetPositions(initLaserPositions);
        laser.startWidth = laserWidth;
        laser.endWidth = laserWidth;
	}
	
	void Update () {
        ShootLaser(laserStart.transform.position, transform.rotation * Vector3.right, greenMaterial);
	}

    void ShootLaser(Vector3 targetPos, Vector3 dir, Material idleMat)
    {
        RaycastHit2D hit = Physics2D.Raycast(targetPos, dir, laserMaxLength, 1 << LayerMask.NameToLayer("Wall"));
        Vector3 endPos = targetPos + (laserMaxLength * dir);
        laser.material = idleMat;

        if (hit.collider != null)
        {
            endPos = hit.point;
            playerSpotted = false;
        }
        RaycastHit2D hitPlayer = Physics2D.Raycast(targetPos, dir, Vector3.Distance(targetPos, endPos), 1 << LayerMask.NameToLayer("Player1"));

        if (hitPlayer.collider != null) {
            playerObject = hitPlayer.collider.gameObject;
            endPos = hitPlayer.transform.position;
            laser.material = redMaterial;
            playerSpotted = true;
            GameObject.Find("Alert").GetComponent<Alert>().AlertOn = true;
        } else {
            playerSpotted = false;
        }
        
        laser.SetPosition(0, targetPos);
        laser.SetPosition(1, endPos);
    }
}
