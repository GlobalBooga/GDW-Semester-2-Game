using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class OptionsMenu : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private AudioMixer Master;
    [SerializeField] private AudioMixer SFX;
    [SerializeField] private AudioMixer Music;
    [SerializeField] private AudioSource MasterSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource MusicSource;

    [Header("Gameplay Settings")]
    [SerializeField] private Slider controllerSensSlider;
    public int mainControllerSen = 10;
    [SerializeField] private Slider difficultySlider;
    public int difficulty = 0; 
    private void Start()
    {
        Master.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", 1) * 20));
        SFX.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", 1) * 20));
        Music.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1) * 20));
    }
    public void ChangeMasterVolume(float Value)
    {
        Master.SetFloat("MasterVolume", Mathf.Log10(Value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", Value);
        PlayerPrefs.Save();
    }
    public void ChangeSFXVolume(float Value)
    {
        SFX.SetFloat("SFXVolume", Mathf.Log10(Value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", Value);
        PlayerPrefs.Save();
    }
    public void ChangeMusicVolume(float Value)
    {
        Music.SetFloat("MusicVolume", Mathf.Log10(Value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", Value);
        PlayerPrefs.Save();
    }

    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);

        Player p = LevelManager.instance.GetPlayer();
        GameData gd = LevelManager.instance.LoadGameData();

        // check if player is valid, if not, then there is no player - meaning we are in the main menu level.
        if (p)
        {
            p.gamepadRotSpeed = mainControllerSen;
        }

        gd.gpRotSpeed = mainControllerSen;

        LevelManager.instance.Save(gd);
    }
}
  

   
