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
    public Text ControllerSensText;
    public Text DifficultyText;

    [Header("Gameplay Settings")]
    [SerializeField] private Slider controllerSensSlider;
    public int mainControllerSen = 10;
    [SerializeField] private Slider difficultySlider;

    private void Start()
    {
        // get sound settings
        MasterSlider.value = PlayerPrefs.GetFloat("MasterVol", 20);
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVol", -20);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVol", -35);

        // get game settings
        controllerSensSlider.value = PlayerPrefs.GetFloat("ControllerSens", 350f);
        difficultySlider.value = PlayerPrefs.GetInt("Difficulty", 1);

        // set stuff
        MasterVolNum.text = Mathf.RoundToInt(MasterSlider.value + 80).ToString();
        MusicVolNum.text = Mathf.RoundToInt(MusicSlider.value + 80).ToString();
        SFXVolNum.text = Mathf.RoundToInt(SFXSlider.value + 80).ToString();
        ControllerSensText.text = Mathf.RoundToInt(controllerSensSlider.value).ToString();
        DifficultyText.text = difficultySlider.value == 0 ? "Easy" : "Normal";

        // set sound
        MainMixer.SetFloat("MasterVol", MasterSlider.value);
        MainMixer.SetFloat("MusicVol", MusicSlider.value);
        MainMixer.SetFloat("SFXVol", SFXSlider.value);
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
        ControllerSensText.text = mainControllerSen.ToString();
        PlayerPrefs.SetFloat("ControllerSens", controllerSensSlider.value);
    }

    public void SetDifficulty()
    {
        int difficulty = Mathf.RoundToInt(difficultySlider.value);
        PlayerPrefs.SetInt("Difficulty", difficulty);

        DifficultyText.text = difficulty == 0 ? "Easy" : "Normal";

        // if we are not in main menu
        if (LevelManager.instance)
        {
            LevelManager.instance.isEasyMode = difficulty == 0;
        }
    }
}

  

   
