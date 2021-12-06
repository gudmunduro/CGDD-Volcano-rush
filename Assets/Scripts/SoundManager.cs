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
    Step,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip[] swipeSounds;
    public AudioClip[] hitSounds;
    public AudioClip[] gruntSounds;
    public AudioClip[] boneSounds;
    public AudioClip[] tumbleSounds;
    public AudioClip[] stepSounds;

    public AudioClip deathSound;
    public AudioClip guardSound;
    public AudioClip jumpSound;
    public AudioClip jumpSound2;

    public AudioSource swipePlayer;
    public AudioSource hitPlayer;
    public AudioSource gruntPlayer;
    public AudioSource bonePlayer;
    public AudioSource tumblePlayer;
    public AudioSource stepPlayer;

    public AudioSource soloPlayer;
    private float soloVolume;
    public AudioSource BGPlayer;

    private void Awake()
    {
        instance = this;
        soloVolume = soloPlayer.volume;
    }

    public void PlayDeath()
    {
        soloPlayer.clip = deathSound;
        soloPlayer.volume = soloVolume;
        soloPlayer.Play();
    }

    public void PlayGuard()
    {
        soloPlayer.clip = guardSound;
        soloPlayer.volume = soloVolume;
        soloPlayer.Play();
    }

    public void PlayJump(bool grounded)
    {
        if (grounded)
        {
            soloPlayer.clip = jumpSound;
            soloPlayer.volume = soloVolume/4f;
        }
        else
        {
            soloPlayer.clip = jumpSound2;
            soloPlayer.volume = soloVolume;
        }
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
            case SoundType.Step:
                clips = stepSounds;
                player = stepPlayer;
                break;
        }
        player.clip = clips[(int)UnityEngine.Random.Range(0, clips.Length)];
        player.Play();
    }

    public float PlayPos(SoundType soundType)
    {
        AudioSource player = swipePlayer;
        switch (soundType)
        {
            case SoundType.Swipe:
                player = swipePlayer;
                break;
            case SoundType.Hit:
                player = hitPlayer;
                break;
            case SoundType.Grunt:
                player = gruntPlayer;
                break;
            case SoundType.Bone:
                player = bonePlayer;
                break;
            case SoundType.Tumble:
                player = tumblePlayer;
                break;
            case SoundType.Step:
                player = stepPlayer;
                break;
        }

        return player.time > 0 ? player.time : Mathf.Infinity;
    }


}
