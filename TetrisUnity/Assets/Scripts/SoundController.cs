using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : Singleton<SoundController>
{
    public AudioSource audioSource;
    public AudioClip moveSound;
    public AudioClip placeSound;
    public AudioClip scoreSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayMove()
    {
        audioSource.pitch = 1;
        audioSource.PlayOneShot(moveSound);
    }
    public void PlayPlacePiece()
    {
        audioSource.pitch = 1;
        audioSource.PlayOneShot(placeSound);
    }
    public void PlayScore(int rowNumb)
    {
        float curPitch = 1 + (rowNumb * 0.1f);
        audioSource.pitch = curPitch;
        audioSource.PlayOneShot(scoreSound);
    }
}