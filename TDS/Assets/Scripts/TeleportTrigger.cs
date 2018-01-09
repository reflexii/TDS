using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1")) {
            GameObject.Find("VIP").transform.position = new Vector3(52.78f, -39f, 0f);
            GameObject.Find("VIPSprite").GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
