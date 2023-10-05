using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip clip_jump;
    [SerializeField] private AudioClip clip_died;
    [SerializeField] private AudioClip clip_victory;

    private bool sfxOn = false;
    public static SoundManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
        sfxOn = SaveInfo.GetInstance().GetSfxOn();
    }

    public void PlayJump()
    {
        if (!sfxOn)
            return;

        src.Stop();
        src.PlayOneShot(clip_jump, 1.25f);
    }

    public void PlayDied()
    {
        if (!sfxOn)
            return;

        src.Stop();
        src.PlayOneShot(clip_died, 1.25f);
    }

    public void PlayVictory()
    {
        if (!sfxOn)
            return;

        src.Stop();
        src.PlayOneShot(clip_victory, 0.8f);
    }


}
