using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private AudioMixer Master;
    [SerializeField]
    private AudioMixer SFX;
    [SerializeField]
    private AudioMixer Music;
    [SerializeField]
    private AudioSource AudioSource;

    private void Start()
    {
        Master.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", 1) * 20));
        SFX.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", 1) * 20));
        Music.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1) * 20));
    }
    public void OnChangeSlider(float Value)
    {
        Master.SetFloat("MasterVolume", Mathf.Log10(Value) * 20);
        SFX.SetFloat("SFXVolume", Mathf.Log10(Value) * 20);
        Music.SetFloat("MusicVolume", Mathf.Log10(Value) * 20);

        PlayerPrefs.SetFloat("MasterVolume", Value);
        PlayerPrefs.Save();
        PlayerPrefs.SetFloat("SFXVolume", Value);
        PlayerPrefs.Save();
        PlayerPrefs.SetFloat("MusicVolume", Value);
        PlayerPrefs.Save();
    }
}
  

   
