using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance = null;

    [SerializeField] private AudioSource bgmSource;
    
    [Header("GAME")]
    [SerializeField] private AudioClip[] paperFlipClips;
    [SerializeField] private AudioSource[] paperFlipSources;
    [SerializeField] private AudioClip[] deathClips;
    [SerializeField] private AudioSource[] deathSources;
    [SerializeField] private AudioSource torchSource;
    [SerializeField] private AudioSource outroRumbleSource;
    [SerializeField] private AudioSource outroBoomSource;
    
    [Header("MENU")]
    [SerializeField] private AudioSource buttonPressSource;
    [SerializeField] private AudioSource menuRumbleSource;
    
    int paperFlipIndex = 0;
    int deathIndex = 0;
    private float numTreesGrown = 0f;
    
    private void Awake()
    {
        Instance = this;
    }

    public void PlayPaperFlipAudio(bool isFlippingAlive, Vector3 audioPosition)
    {
        var flipSource = paperFlipSources[paperFlipIndex];
        if (flipSource.isPlaying) { return; }

        flipSource.transform.position = audioPosition;
        var flipClip = paperFlipClips[Random.Range(0, paperFlipClips.Length)];
        flipSource.clip = flipClip;
        flipSource.pitch = isFlippingAlive ? Random.Range(1f, 1.2f) : Random.Range(0.8f, 1f);
        flipSource.Play();
        paperFlipIndex = (paperFlipIndex + 1) % paperFlipSources.Length;
    }

    public void PlayTreeGrow1(AudioSource treeSource, AudioClip treeClip)
    {
        //Grow
        var maxVolume = treeSource.volume;
        treeSource.spatialBlend = 0f;
        treeSource.clip = treeClip;
        treeSource.pitch = 0f;
        treeSource.volume = maxVolume * 0.5f;
        treeSource.DOPitch(1f, 3f).SetEase(Ease.OutQuad);
        treeSource.DOFade(maxVolume, 3f).SetEase(Ease.OutQuad);
        treeSource.Play();
    }
    
    public void PlayTreeGrow2(AudioSource treeSource, AudioClip treeClip)
    {
        //Sproing
        treeSource.Stop();
        treeSource.clip = treeClip;
        treeSource.pitch += (numTreesGrown * 0.1f);
        treeSource.Play();
        numTreesGrown++;
    }

    public void IncreaseBGM()
    {
        bgmSource.volume += 0.1f;
    }

    public void StopBGM(float fadeOutTime)
    {
        bgmSource.DOFade(0f, fadeOutTime).SetEase(Ease.InSine).OnComplete(bgmSource.Stop);
    }

    public void PlayUnitDied(Vector3 unitPosition)
    {
        PlayAllDeathClips(unitPosition);
    }

    private void PlayAllDeathClips(Vector3 unitPosition)
    {
        var randomPitch = Random.Range(1.1f, 1.2f);
        foreach (var deathClip in deathClips)
        {
            var deathSource = deathSources[deathIndex];
            deathSource.Stop();
            deathSource.transform.position = unitPosition;
            deathSource.clip = deathClip;
            deathSource.pitch = randomPitch;
            deathSource.Play();
            deathIndex = (deathIndex + 1) % deathSources.Length;
        }
    }

    public void PlayTorchLit()
    {
        torchSource.Play();
    }

    public void PlayRumble(float fadeInTime)
    {
        outroRumbleSource.volume = 0f;
        outroRumbleSource.DOFade(1f, fadeInTime).SetEase(Ease.Linear);
        outroRumbleSource.Play();
    }
    
    public void StopRumble(float fadeOutTime)
    {
        outroRumbleSource.DOFade(0f, fadeOutTime).SetEase(Ease.Linear).OnComplete(outroRumbleSource.Stop);
    }

    public void FadeRumblePitch(float destinationPitch, float fadeTime)
    {
        outroRumbleSource.DOPitch(destinationPitch, fadeTime);
    }
    
    public void PlayOutroBoom()
    {
        outroBoomSource.Play();
    }

    #region Menu

    public void PlayButtonPress()
    {
        buttonPressSource.pitch = Random.Range(0.9f, 1f);
        buttonPressSource.Play();
    }

    public void FadeMenuRumble(float fadeOutTime)
    {
        if (menuRumbleSource == null) { return; }
        
        menuRumbleSource.DOFade(0f, fadeOutTime).SetEase(Ease.InSine).OnComplete(menuRumbleSource.Stop);
    }
    #endregion
}
