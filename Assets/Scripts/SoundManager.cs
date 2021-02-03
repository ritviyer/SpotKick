using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField]AudioSource backgroundSound;
    [SerializeField]AudioSource cheerSound;
    [SerializeField]AudioSource kickSound;


    [SerializeField]Sprite musicOn;
    [SerializeField]Sprite musicOff;
    [SerializeField]Button musicButton;
    private void Awake()
    {
        ChangeMusicIcon();
    }
    private void OnEnable()
    {
        EventManager.onStartGame += PlayBackgroundMusic;
        EventManager.onLevelComplete += PlayCheerMusic;
    }
    private void OnDisable()
    {
        EventManager.onStartGame -= PlayBackgroundMusic;
        EventManager.onLevelComplete -= PlayCheerMusic;
    }
    public void PlayBackgroundMusic()
    {
        cheerSound.Stop();
        //backgroundSound.Play();
        IncreaseBackgroundVolume();
    }
    public void PlayCheerMusic()
    {
        DecreaseBackgroundVolume();
        cheerSound.Play();
        Invoke("IncreaseBackgroundVolume", 8f);
    }
    public void PlayKickMusic()
    {
        DecreaseBackgroundVolume(0.5f);
        kickSound.Play();
        Invoke("IncreaseBackgroundVolume", 1f);
    }
    public void StopBackgroundMusic()
    {
        backgroundSound.Pause();
    }
   
    void IncreaseBackgroundVolume()
    {
        backgroundSound.volume = 1f;
    }
    void DecreaseBackgroundVolume(float dec = 0.05f)
    {
        backgroundSound.volume = dec;
    }
    public void ToPlayMusic()
    {
        int playMusic = PlayerPrefs.GetInt("PlayMusic", 1);

        if (playMusic == 1)
            PlayerPrefs.SetInt("PlayMusic", 0);
        
        else if (playMusic == 0)
            PlayerPrefs.SetInt("PlayMusic", 1);

        ChangeMusicIcon();
    }
    void ChangeMusicIcon()
    {
        int playMusic = PlayerPrefs.GetInt("PlayMusic",1);

        if (playMusic == 1)
        {
            musicButton.image.sprite = musicOn;
            backgroundSound.mute = false;
            cheerSound.mute = false;
            kickSound.mute = false;
        }
        else if (playMusic == 0)
        {
            musicButton.image.sprite = musicOff;
            backgroundSound.mute = true;
            cheerSound.mute = true;
            kickSound.mute = true;
        }
    }
}
