using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource MusicPlayer;
    public AudioSource SoundEffectPlayer;
    public void PlayMusic(AudioClip ac,float Volume=1)
    {
        MusicPlayer.clip = ac;
        MusicPlayer.volume = Volume;
        MusicPlayer.Play();
    }

    public void PlaySE(AudioClip ac)
    {
        SoundEffectPlayer.clip = ac;
        SoundEffectPlayer.Play();
    }
}
