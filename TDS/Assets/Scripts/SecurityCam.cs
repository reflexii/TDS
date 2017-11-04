using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCam : MonoBehaviour {

    public float laserWidthStart = 0.2f;
    public float laserWidthEnd;
    public float laserMaxLength = 5f;
    public Material greenMaterial;
    public Material redMaterial;
    public Material yellowMaterial;
    public bool playerSpotted = false;
    [HideInInspector]
    public GameObject playerObject;

    private LineRenderer laser;
    private GameObject laserStart;
    private AnimationCurve curve;

    void Start () {
        laser = GetComponent<LineRenderer>();
        laserStart = transform.parent.Find("LaserStart").gameObject;
        Vector3[] initLaserPositions = new Vector3[2] { transform.position, transform.position };
        laser.SetPositions(initLaserPositions);
        laser.startWidth = laserWidthStart;
        laser.endWidth = laser.endWidth;

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
        RaycastHit2D hitPlayer2 = Physics2D.Raycast(targetPos, Quaternion.Euler(new Vector3(0f, 0f, 12f)) * dir, Vector3.Distance(targetPos, endPos), 1 << LayerMask.NameToLayer("Player1"));
        RaycastHit2D hitPlayer3 = Physics2D.Raycast(targetPos, Quaternion.Euler(new Vector3(0f, 0f, 12f)) * dir, Vector3.Distance(targetPos, endPos), 1 << LayerMask.NameToLayer("Player1"));

        if (hitPlayer.collider != null) {
            playerObject = hitPlayer.collider.gameObject;
            endPos = hitPlayer.transform.position;
            laser.material = redMaterial;
            playerSpotted = true;
            GameObject.Find("Alert").GetComponent<Alert>().AlertOn = true;
        }

        if (hitPlayer2.collider != null) {
            playerObject = hitPlayer2.collider.gameObject;
            endPos = hitPlayer2.transform.position;
            laser.material = redMaterial;
            playerSpotted = true;
            GameObject.Find("Alert").GetComponent<Alert>().AlertOn = true;
        }

        if (hitPlayer3.collider != null) {
            playerObject = hitPlayer3.collider.gameObject;
            endPos = hitPlayer3.transform.position;
            laser.material = redMaterial;
            playerSpotted = true;
            GameObject.Find("Alert").GetComponent<Alert>().AlertOn = true;
        }

        if (hitPlayer3.collider == null && hitPlayer2.collider == null && hitPlayer.collider == null) {
            playerSpotted = false;
        }

        ScaleWidth(targetPos, endPos);

        laser.SetPosition(0, targetPos);
        laser.SetPosition(1, endPos);
    }

    void ScaleWidth(Vector3 start, Vector3 end) {
        float dist = Vector3.Distance(start, end);
        float reduction = laserWidthEnd - laserWidthStart;

        float endWidth = (reduction * (dist / laserMaxLength)) + (laserWidthStart/2f);
        laser.endWidth = endWidth * laser.widthMultiplier;
    }

    /*
    private void OnDrawGizmos() {
        Gizmos.DrawRay(laserStart.transform.position, transform.rotation * Quaternion.Euler(new Vector3(0f, 0f, 12f)) * Vector3.right * laserMaxLength);
        Gizmos.DrawRay(laserStart.transform.position, transform.rotation * Quaternion.Euler(new Vector3(0f, 0f, -12f)) * Vector3.right * laserMaxLength);
        Gizmos.DrawRay(laserStart.transform.position, transform.rotation * Vector3.right * laserMaxLength);
    }
    */
}
