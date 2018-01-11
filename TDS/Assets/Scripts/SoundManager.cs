using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public List<AudioClip> audioList;
    public List<string> audioListString;
    public List<GameObject> disableAudioList;
    private ObjectPooler op;
    private GameManager gm;
	void Awake () {
        gm = gameObject.GetComponent<GameManager>();
        op = GetComponent<ObjectPooler>();
        audioListString = new List<string>();
        disableAudioList = new List<GameObject>();

        foreach (AudioClip audio in audioList) {
            audioListString.Add(audio.name);
        }
	}

    public void PlaySound(string soundFileName, bool randomizePitch = false, float pitchMin = 0.92f, float pitchMax = 1.08f) {
        if (audioListString.Contains(soundFileName)) {
            
            GameObject audio = op.GetPooledAudio();
            audio.SetActive(true);
            AudioSource source = audio.GetComponent<AudioSource>();
            source.clip = audioList[audioListString.IndexOf(soundFileName)];
            source.volume = (gm.mms.sfxVolume/100f);
            if (gm.mms.soundMuted) {
                source.volume = 0f;
            }

            if (randomizePitch) {
                RandomizePitch(source, pitchMin, pitchMax);
            }

            source.Play();

            disableAudioList.Add(audio);
        }
    }

    private void Update() {
        DisableGameObjectsWhenSoundIsntPlaying();
    }

    void DisableGameObjectsWhenSoundIsntPlaying() {
        for (int i = disableAudioList.Count-1; i >= 0; i--) {
            if (!disableAudioList[i].GetComponent<AudioSource>().isPlaying) {
                op.audioList[op.audioList.IndexOf(disableAudioList[i])].SetActive(false);
                disableAudioList.Remove(disableAudioList[i]);
            }
        }
    }
    
    private void RandomizePitch(AudioSource a, float pitchMin, float pitchMax) {
        float randomized = Random.Range(pitchMin, pitchMax);

        a.pitch = randomized;
    }
}
