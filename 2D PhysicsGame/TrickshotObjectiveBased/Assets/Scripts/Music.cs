using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

    //Some variables to store the audio source and clip - KT

    private AudioSource asAudioPlayer;

    public AudioClip[] audioClips;

    // Use this for initialization
    void Start () {
        //Get the audio source and set the clip - KT
        asAudioPlayer = this.gameObject.GetComponent<AudioSource>();
        asAudioPlayer.clip = audioClips[Random.Range(0, 3)];
        asAudioPlayer.PlayOneShot(asAudioPlayer.clip, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
        //Check if the audio is still playing. If it isn't then play a new song - KT
        if (!asAudioPlayer.isPlaying)
        {
            asAudioPlayer.clip = audioClips[Random.Range(0, 3)];
            asAudioPlayer.PlayOneShot(asAudioPlayer.clip, 0.5f);
        }
	}
}
