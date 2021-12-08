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
    Eat,
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
    public AudioClip[] eatSounds;

    public AudioClip deathSound;
    public AudioClip guardSound;
    public AudioClip jumpSound;
    public AudioClip jumpSound2;
    public AudioClip slideSound;

    public AudioSource swipePlayer;
    public AudioSource hitPlayer;
    public AudioSource gruntPlayer;
    public AudioSource bonePlayer;
    public AudioSource tumblePlayer;
    public AudioSource stepPlayer;
    public AudioSource eatPlayer;

    public AudioSource soloPlayer;
    private float soloVolume;

    public AudioSource BGPlayer;

    public AudioSource heartPlayer;

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
        StopSolo();
        soloPlayer.clip = guardSound;
        soloPlayer.volume = soloVolume;
        soloPlayer.Play();
    }

    public void PlayJump(bool grounded)
    {
        if (grounded)
        {
            soloPlayer.clip = jumpSound;
            soloPlayer.volume = soloVolume/2f;
        }
        else
        {
            soloPlayer.clip = jumpSound2;
            soloPlayer.volume = soloVolume;
        }
        soloPlayer.Play();
    }

    public void PlaySlide()
    {
        soloPlayer.clip = slideSound;
        soloPlayer.volume = soloVolume;
        soloPlayer.Play();
    }

    public void StopSolo()
    {
        soloPlayer.Stop();
    }

    public bool PlayingSlide()
    {
        return soloPlayer.isPlaying && soloPlayer.clip == slideSound;
    }

    public void HeartVolume(float volume)
    {
        if (!heartPlayer.isPlaying)
            heartPlayer.Play();
        heartPlayer.volume = volume;
    }

    public void PlaySound(SoundType type)
    {
        AudioClip[] clips;
        AudioSource player;
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
            case SoundType.Eat:
                clips = eatSounds;
                player = eatPlayer;
                break;
            default:
                clips = swipeSounds;
                player = swipePlayer;
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
