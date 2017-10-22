using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestroyer : MonoBehaviour {

    private ParticleSystem ps;

	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	void Update () {
        if (ps != null) {
            if (!ps.IsAlive()) {
                Destroy(gameObject);
            }
        }
	}
}
