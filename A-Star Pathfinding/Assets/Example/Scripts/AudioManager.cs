using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sound {
    public AudioClip audioClip;
    public float vol;
    public bool pitchChange;
}

public class AudioManager : MonoBehaviour {

    public Sound[] snds;
    internal List<AudioSource> AudioSources = new List<AudioSource>();
    public static AudioManager am;
    public Dictionary<AudioClip, int> playedRecently = new Dictionary<AudioClip, int>();

    int currentSourceIndex;

	void Start () {
        for (int i = 0; i < 16; i++) {
            AudioSources.Add(gameObject.AddComponent<AudioSource>());
        }
        am = this;
        StartCoroutine(ResetTimesPlayedRecently());
	}

    public void PlaySound(int index) {
        if (CanPlaySound(snds[index].audioClip)) {
            var clip = snds[index].audioClip;
            if (snds[index].pitchChange) AudioSources[currentSourceIndex].pitch = Random.Range(.8f, 1.2f);
            else AudioSources[currentSourceIndex].pitch = 1;
            AudioSources[currentSourceIndex].PlayOneShot(clip, snds[index].vol);
            IncrementAudioSourceIndex();
        }
    }

    public void PlayClip(AudioClip ac, bool pitchShift = true, float vol = 1) {
        if (CanPlaySound(ac)) {
            if (pitchShift) AudioSources[currentSourceIndex].pitch = Random.Range(.8f, 1.2f);
            else AudioSources[currentSourceIndex].pitch = 1;
            AudioSources[currentSourceIndex].PlayOneShot(ac, vol);
            IncrementAudioSourceIndex();
        }
    }

    public void IncrementAudioSourceIndex() {
        currentSourceIndex++;
        if (currentSourceIndex == AudioSources.Count) currentSourceIndex = 0;
    }

    public bool CanPlaySound(AudioClip clip) {
        int timesPlayed = 0;
        if (playedRecently.TryGetValue(clip, out timesPlayed)) {
            playedRecently[clip]++;
            return (timesPlayed < 5);
        } else {
            playedRecently.Add(clip, 1);
            return true;
        }
    }

    public IEnumerator ResetTimesPlayedRecently() {
        while (true) {
            playedRecently.Clear();
            yield return new WaitForSeconds(.3f);
        }
    }

}
