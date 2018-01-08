using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMapButtonScript : MonoBehaviour {

    
    private GameObject buttonSprite;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet")) {
           
        }
    }
}
