using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class OptionsMenu : MonoBehaviour
{
    [Header("Volume")]
    public AudioMixer MainMixer;
    public Slider MasterSlider;
    public Slider MusicSlider;
    public Slider SFXSlider;
    public Text MasterVolNum;
    public Text MusicVolNum;
    public Text SFXVolNum;

    [Header("Gameplay Settings")]
    [SerializeField] private Slider controllerSensSlider;
    public int mainControllerSen = 10;
    [SerializeField] private Slider difficultySlider;
    public int difficulty = 0; 
    private void Start()
    {
        float vol = 0f;
        MainMixer.GetFloat("MasterVol", out vol);
        MasterSlider.value = vol;
        MainMixer.GetFloat("MusicVol", out vol);
        MusicSlider.value = vol;
        MainMixer.GetFloat("SFXVol", out vol);
        SFXSlider.value = vol;

        MasterVolNum.text = Mathf.RoundToInt(MasterSlider.value + 80).ToString();
        MusicVolNum.text = Mathf.RoundToInt(MusicSlider.value + 80).ToString();
        SFXVolNum.text = Mathf.RoundToInt(SFXSlider.value + 80).ToString();
    }
    public void SetMasterVol()
    {
        MasterVolNum.text = Mathf.RoundToInt(MasterSlider.value + 80).ToString();

        MainMixer.SetFloat("MasterVol", MasterSlider.value);

        PlayerPrefs.SetFloat("MasterVol", MasterSlider.value);
    }
    public void SetMusicVol()
    {
        MusicVolNum.text = Mathf.RoundToInt(MusicSlider.value + 80).ToString();

        MainMixer.SetFloat("MusicVol", MusicSlider.value);

        PlayerPrefs.SetFloat("MusicVol", MusicSlider.value);
    }
    public void SetSFXVol()
    {
        SFXVolNum.text = Mathf.RoundToInt(SFXSlider.value + 80).ToString();

        MainMixer.SetFloat("SFXVol", SFXSlider.value);

        PlayerPrefs.SetFloat("SFXVol", SFXSlider.value);
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
    public void SetDifficulty(float difficultySliderAmount)
    {
        difficulty = Mathf.RoundToInt(difficultySliderAmount);

        if (difficulty == 0)
        {

        }

        if (difficulty == 1)
        {

        }
    }
}
  

   
