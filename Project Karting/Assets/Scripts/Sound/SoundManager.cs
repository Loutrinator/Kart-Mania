using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip raceMusic;
    [SerializeField] private AudioClip clicSound;
    [SerializeField] private AudioClip validateSound;
    [SerializeField] private AudioClip backSound;

    [SerializeField] private AudioSource musicSource;

    [Range(0f,1f)]
    [SerializeField] private float musicVolume;
    [SerializeField] private float fadeSpeed;
    

    public static SoundManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //cameras = new List<ShakeTransform>();
            DontDestroyOnLoad(this);
        }
        musicSource.volume = 0f;
    }


    public void PlayMainMenuMusic()
    {
        musicSource.clip = mainMenuMusic;
        musicSource.Play();
        DOTween.To(() => musicSource.volume,value => musicSource.volume = value, musicVolume, fadeSpeed);
    }
    public void PlayRaceMusic()
    {
        musicSource.clip = raceMusic;
        musicSource.Play();
        DOTween.To(() => musicSource.volume,value => musicSource.volume = value, musicVolume, fadeSpeed);
    }
    public void FadeOutMusic()
    {
        DOTween.To(() => musicSource.volume, value => musicSource.volume = value, 0, fadeSpeed).OnComplete(() => {musicSource.Stop();});
    }
}
