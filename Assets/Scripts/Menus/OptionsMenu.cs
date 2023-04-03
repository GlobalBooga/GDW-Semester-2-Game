using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
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
        MasterSlider.value = PlayerPrefs.GetFloat("MasterVol");
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVol");

        controllerSensSlider.value = PlayerPrefs.GetFloat("ControllerSens");

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

    public void SetControllerSen()
    {
        mainControllerSen = Mathf.RoundToInt(controllerSensSlider.value);

        PlayerPrefs.SetFloat("ControllerSens", controllerSensSlider.value);
    }

    public void SetDifficulty()
    {
        PlayerPrefs.SetInt("Difficulty", difficulty);

        // if we are not in main menu
        if (LevelManager.instance)
        {
            LevelManager.instance.isEasyMode = difficulty == 0;
        }
    }
}

  

   
