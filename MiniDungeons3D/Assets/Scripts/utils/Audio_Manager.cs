using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class Audio_Manager : MonoBehaviour
{
    public AudioClip[] musicClips;
    public AudioClip[] sfxClips;
    private int currentTrack { get; }
    public AudioSource source;
    public AudioSource sfx;
    // Start is called before the first frame update
    void Start()
    {
        // PLAY MUSIC

        source.clip = musicClips[0];
        source.Play();
    }

    public void PlayMusic(int trackID)
    {
        if (trackID > musicClips.Length - 1)
        {
            return;
        }

        if (source.isPlaying)
        {
            source.Stop();
        }


        // CHANGE CLIP TO TRACKID
        source.clip = musicClips[trackID];
        source.Play();

    }

    public void PlaySFX(int soundID)
    {
        if (soundID > sfxClips.Length - 1)
        {
            return;
        }

        if (sfx.isPlaying)
        {
            sfx.Stop();
        }


        // CHANGE CLIP TO TRACKID
        sfx.clip = sfxClips[soundID];
        sfx.Play();
    }

    public void ToggleMute()
    {
        if (source.volume == 0.0f)
        {
            source.volume = 100.0f;
            sfx.volume = 100.0f;
        }
        else
        {
            source.volume = 0.0f;
            sfx.volume = 0.0f;
        }
    }


}
