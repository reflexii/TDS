using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasSoundScript : MonoBehaviour {

    private GameObject player;
    private GameManager gm;
    private AudioSource source;
    
    public bool toggleSound = false;

	void Awake () {
        player = GameObject.Find("Player");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        source = GetComponent<AudioSource>();
        source.volume = gm.mms.sfxVolume;
	}

	void Update () {
        if (toggleSound) {
            if (!source.isPlaying) {
                source.Play();
            }
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance > 20f) {
                source.volume = 0f;
            } else {
                float value = 1 - (distance / 20f);
                value *= gm.mms.sfxVolume / 100f;
                source.volume = value;
            }
        }
	}
}
