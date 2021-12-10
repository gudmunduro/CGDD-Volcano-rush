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
    TumbleAir,
    Step,
    Eat,
    ArrowShoot,
    ArrowImpact,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip[] swipeSounds;
    public AudioClip[] hitSounds;
    public AudioClip[] gruntSounds;
    public AudioClip[] boneSounds;
    public AudioClip[] tumbleSounds;
    public AudioClip[] tumbleAirSounds;
    public AudioClip[] stepSounds;
    public AudioClip[] eatSounds;
    public AudioClip[] arrowShootSounds;
    public AudioClip[] arrowImpactSounds;

    public AudioClip deathSound;
    public AudioClip guardSound;
    public AudioClip jumpSound;
    public AudioClip jumpSound2;
    public AudioClip slideSound;
    public AudioClip completeSound;
    public AudioClip powerUpSound;
    public AudioClip powerDownSound;

    public AudioSource swipePlayer;
    public AudioSource hitPlayer;
    public AudioSource gruntPlayer;
    public AudioSource bonePlayer;
    public AudioSource tumblePlayer;
    public AudioSource stepPlayer;
    public AudioSource eatPlayer;
    public AudioSource powerPlayer;
    public AudioSource checkpointPlayer;
    public AudioSource arrowShootPlayer;
    public AudioSource arrowImpactPlayer;

    public AudioSource soloPlayer;
    private float soloVolume;

    public AudioSource BGPlayer;
    private float BGVolume;

    public AudioSource agroPlayer;

    public AudioSource heartPlayer;

    public bool fading = false;

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

    public void PlayPower(bool up)
    {
        powerPlayer.clip = up ? powerUpSound : powerDownSound;
        powerPlayer.Play();
    }

    public void PlayComplete()
    {
        soloPlayer.clip = completeSound;
        soloPlayer.volume = soloVolume;
        soloPlayer.Play();

        StartCoroutine(FadeOutBGM());
        HeartVolume(0);
    }

    public void StopSolo()
    {
        soloPlayer.Stop();
    }

    public void ArrowShootVolume(float volume)
    {
        arrowShootPlayer.volume = volume;
    }

    public void ArrowImpactVolume(float volume)
    {
        arrowImpactPlayer.volume = volume;
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

    public void PlayCheckpoint()
    {
        checkpointPlayer.Play();
    }

    public IEnumerator FadeOutBGM()
    {
        float time = 0;
        float startValue = BGPlayer.volume;

        while (time < 1)
        {
            BGPlayer.volume = Mathf.Lerp(startValue, 0f, time);
            time += 0.005f;
            yield return null;
        }
        BGPlayer.volume = 0f;
        BGPlayer.Stop();
    }

    public IEnumerator FadeAgro(float endValue, float duration)
    {
        fading = true;
        float time = 0;
        float startValue = agroPlayer.volume;

        while (time < duration)
        {
            agroPlayer.volume = Mathf.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        fading = false;
        agroPlayer.volume = endValue;
    }

    public string StupidText(int submitted)
    {
        string _texty = "";
        switch (submitted)
        {
            case 1:
                _texty = "You have already submitted!";
                break;
            case 2:
                _texty = "You don't have to worry, it's saved!";
                break;
            case 3:
                _texty = "I'm 100% sure, just stop pressing submit!";
                break;
            case 4:
                _texty = "What did I just say!?";
                break;
            case 5:
                _texty = "Are you serious!?";
                break;
            case 6:
                _texty = "Do you really think you're making sure of anything at this point!?";
                break;
            case 7:
                _texty = "Hey, like I've said, your score has been saved to our servers! Relax!";
                break;
            case 8:
                _texty = "This is getting ridiculous, just go do something else!";
                break;
            case 9:
                _texty = "Like TRYING TO GET A BETTER SCORE!!!";
                break;
            case 10:
                _texty = "Or not, just keep pressing submit since you're sooo interested in what I have to say!";
                break;
            case 11:
                _texty = "The poor developer who had to add all of this mind numming filler text, and for what!?";
                break;
            case 12:
                _texty = "Just for you get something out of it!? You're actually starting to annoy me!";
                break;
            case 13:
                _texty = "That's it, I'm done!";
                break;
            case 14:
                _texty = "No, now I'm actually done!";
                break;
            case 50:
                _texty = "MORE FUNNIES BEYOND THIS POINT!";
                break;
            case 53:
                _texty = "Haha, made you look twice!";
                break;
            case 54:
                _texty = "No but seriously, you should go do something else!";
                break;
            case 1000:
                _texty = "Ok, stop! STOP!!!";
                break;
            case 1001:
                _texty = "You've actually reached 1000, great job!";
                break;
            case 1002:
                _texty = "I'm proud of you, but I'll have to return you to the main menu now!";
                break;
            case 1003:
                _texty = "It's not wise to venture further past this point!";
                break;
            case 1004:
                _texty = "Until next time!";
                break;
            case 1005:
                GameManager.instance.MainMenu();
                break;
            default:
                _texty = "NO MORE FUNNIES BEYOND THIS POINT!";
                break;
        }
        return _texty;
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
            case SoundType.TumbleAir:
                clips = tumbleAirSounds;
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
            case SoundType.ArrowShoot:
                clips = arrowShootSounds;
                player = arrowShootPlayer;
                break;
            case SoundType.ArrowImpact:
                clips = arrowImpactSounds;
                player = arrowImpactPlayer;
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
