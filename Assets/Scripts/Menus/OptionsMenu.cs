using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer Master;
    public AudioMixer SFX;
    public AudioMixer Music;

    public void SetMasterVolume(float volume)
    {
        Master.SetFloat("volume", volume);
    }
    public void SetSFXVolume(float volume)
    {
        SFX.SetFloat("volume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        Music.SetFloat("volume", volume);
    }
}
