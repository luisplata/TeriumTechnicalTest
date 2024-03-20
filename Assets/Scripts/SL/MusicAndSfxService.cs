using System;
using SL;
using UnityEngine;

public class MusicAndSfxService : MonoBehaviour, ISoundAndMusic
{
    [SerializeField] private AudioClip musicMenu, musicGame;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    private void Start()
    {
        try
        {
            ServiceLocator.Instance.GetService<ISoundAndMusic>();
        }
        catch (Exception e)
        {
            ServiceLocator.Instance.RegisterService<ISoundAndMusic>(this);
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void PlayMusicMenu()
    {
        musicSource.Stop();
        musicSource.clip = musicMenu;
        musicSource.loop = true;
        musicSource.Play();
    }
    
    public void PlayMusicGame()
    {
        musicSource.Stop();
        musicSource.clip = musicGame;
        musicSource.loop = true;
        musicSource.Play();
    }
    
    public void PlaySfx(AudioClip sfx)
    {
        sfxSource.PlayOneShot(sfx);
    }
}

public interface ISoundAndMusic
{
    void PlaySfx(AudioClip sfx);
    void PlayMusicGame();
    void PlayMusicMenu();
}