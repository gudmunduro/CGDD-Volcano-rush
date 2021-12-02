using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Swipe,
    Hit,
    Grunt,
    Bone,
    Tumble,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip[] swipeSounds;
    public AudioClip[] hitSounds;
    public AudioClip[] gruntSounds;
    public AudioClip[] boneSounds;
    public AudioClip[] tumbleSounds;

    public AudioClip deathSound;
    public AudioClip guardSound;

    public AudioSource swipePlayer;
    public AudioSource hitPlayer;
    public AudioSource gruntPlayer;
    public AudioSource bonePlayer;
    public AudioSource tumblePlayer;

    public AudioSource soloPlayer;

    private void Awake()
    {
        instance = this;
    }

    public void PlayDeath()
    {
        soloPlayer.clip = deathSound;
        soloPlayer.Play();
    }

    public void PlayGuard()
    {
        soloPlayer.clip = guardSound;
        soloPlayer.Play();
    }

    public void PlaySound(SoundType type)
    {
        AudioClip[] clips = swipeSounds;
        AudioSource player = swipePlayer;
        switch (type)
        {
            case SoundType.Swipe:
                clips = swipeSounds;
                player = swipePlayer;
                break;
            case SoundType.Hit:
                clips = hitSounds;
                player = hitPlayer;
                break;
            case SoundType.Grunt:
                clips = gruntSounds;
                player = gruntPlayer;
                break;
            case SoundType.Bone:
                clips = boneSounds;
                player = bonePlayer;
                break;
            case SoundType.Tumble:
                clips = tumbleSounds;
                player = tumblePlayer;
                break;
        }
        player.clip = clips[(int)UnityEngine.Random.Range(0, clips.Length)];
        player.Play();
    }
}
