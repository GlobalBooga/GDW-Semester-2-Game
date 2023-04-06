using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public enum Weather
    {
        day_clear,
        day_snow,
        night_clear,
        night_snow,
        max
    }

    [Serializable]
    public struct Scene
    {
        public SceneManager manager;
        public SceneExit exit;
        public GameObject enemyContainer;
        public GameObject gameplayObjectContainer;
        public Transform playerEntrance;
        public Vector2 enterDirection;
        public bool skip;
    }

    public static LevelManager instance;

    [Header("Music")]
    public AudioSource backgroundAudioSource;
    public float onDiedAudioOutSpeed = 50f;

    [Header("Menu")]
    public PauseMenu pauseMenu;

    [Header("DeadScreen")]
    public DeadScreen deadScreen;

    [Header("Crosshair Settings")]
    public Color color = Color.yellow;
    public Image cursor;
    private bool isVisible;

    [Header("Dialogue")]
    public DialogueController dialogueController;

    [Header("General Scene Settings")]
    public bool loop;
    [HideInInspector] public bool isFirst;

    [Header("Scene Objective")]
    public HintController hintController;

    [Header("Weather Settings")]
    public bool enableWeather;
    public float dayClearBrightness = 1f;
    public float daySnowBrightness = 0.8f;
    public float nightClearBrightness = 0.3f;
    public float nightSnowBrightness = 0.1f;
    public Light2D globalLight;
    public GameObject lightsContainer;
    public GameObject snowPrefab;
    private Weather weather;

    [Header("Scene Transition")]
    public Animator screenOverlayAnimator;
    public float transitionTime = 0.2f;
    public const string TRANSITION_ANIM = "SceneTransition";
    public const string TELEPORT_START = "TeleportStart";
    public const string TELEPORT_END = "TeleportEnd";

    [Header("Boss")]
    public string bossTitle = "ANDAROZ THE GREEDY";

    [Header("Scenes")]
    [SerializeField] private List<Scene> orderedScenes;

    // difficulty
    public bool isEasyMode;
    private bool difficultyChanged;

    public Scene CurrentScene => orderedScenes[currentSceneIndex];

    private int currentSceneIndex = 0;
    private bool isQuitting;
    public bool playerFound { get; set; }

    private Player player;

    // for andaroz
    [HideInInspector] public bool startBossBattle;

    private void Awake()
    {
        instance = this;
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (cursor)
        {
            cursor.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            cursor.color = color;
        }

        // automatically update HP when difficulty changes
        if (isEasyMode && player.GetHPComponent().maxHealth == 500)
        {
            player.GetHPComponent().SetEasyMode();

            // also update the lighting
            if (CurrentScene.manager.GetType() == typeof(AmbushBossManager))
            {
                AmbushBossManager a = CurrentScene.manager as AmbushBossManager;
                a.MakeEasy();
            }
            else
            {
                SetGlobalLightAccordingToWeather();
            }

        }
        else if (!isEasyMode && player.GetHPComponent().maxHealth == 2000)
        {
            player.GetHPComponent().SetNormalMode();

            // also update the lighting
            if (CurrentScene.manager.GetType() == typeof(AmbushBossManager))
            {
                AmbushBossManager a = CurrentScene.manager as AmbushBossManager;
                a.MakeNormal();
            }
            else
            {
                SetGlobalLightAccordingToWeather();
            }
        }
    }

    private void OnApplicationQuit()
    {
        ResetProgressFull();
        isQuitting = true;
    }

    private void OnDisable()
    {
        isQuitting = true;
    }

    private void Start()
    {
        isEasyMode = PlayerPrefs.GetInt("Difficulty") == 0;

        SetGlobalLightAccordingToWeather();
        isFirst = true;
        Cursor.visible = false;
        weather = (Weather)UnityEngine.Random.Range(0, (int)Weather.max);

        if (!globalLight)
        {
            enableWeather = false;
        }

        if (orderedScenes.Count == 0) return;

        // disable all scenes besides the first one
        for (int i = 0; i < orderedScenes.Count; i++)
        {
            currentSceneIndex = i;
            orderedScenes[i].manager.DisableScene();
            if (orderedScenes[i].exit) orderedScenes[i].exit.enabled = true;
        }

        orderedScenes[currentSceneIndex = 0].manager.SetScene();
    }

    public void AlertAllEnemiesInCurrentScene(Vector3 alertOrigin)
    {
        if (!orderedScenes[currentSceneIndex].enemyContainer) return;

        Enemy[] enemies = orderedScenes[currentSceneIndex].enemyContainer.GetComponentsInChildren<Enemy>();

        if (enemies == null) return;

        if (enemies.Length > 0)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Alert(alertOrigin);
            }
        }
    }

    public void NextScene()
    {
        playerFound = false;

        // close the current scene
        orderedScenes[currentSceneIndex].manager.DisableScene();

        // if last scene in level and not a looping level
        if (currentSceneIndex == orderedScenes.Count-1 && !loop)
        {
            // end level
            if (player.weapon.GetID() == 4)
            {
                GameData gm = LoadGameData();
                gm.foundAndarozGun = true;
                Debug.Log("foundGun");
                Save(gm);
            }

            // Return to hub
            isQuitting = true;
            screenOverlayAnimator.Play(TELEPORT_START);
            Invoke(nameof(ReturnToHub), 0.25f);

            return;
        }

        // are we within the range
        if (++currentSceneIndex < orderedScenes.Count)
        {
            orderedScenes[currentSceneIndex].manager.SetScene();
        }
        else
        {
            // are all scenes set to skip
            int s = 0;
            foreach (Scene scene in orderedScenes)
            {
                if (scene.skip)
                {
                    s++;
                }
            }
            // if they are
            if (s == orderedScenes.Count)
            {
                Debug.LogWarning("All scenes were set to skip! Staying on scene 1");
                orderedScenes[currentSceneIndex = 0].manager.ForceSetScene();
                return;
            }
            else
            {
                orderedScenes[currentSceneIndex = 0].manager.SetScene();
            }
        }
    }

    public void EnemyDied()
    {
        orderedScenes[currentSceneIndex].manager.ProgressKillAllEnemies();
    }

    public void StartSceneTransition()
    {
        if (screenOverlayAnimator) screenOverlayAnimator.Play(TRANSITION_ANIM);
        if (transitionTime > 0) Invoke(nameof(NextScene), transitionTime);
        else NextScene();
    }

    public void DisablePlayerInput()
    {
        if (!player)
        {
            Debug.LogError("For some reason, LevelManager can't find player");
            return;
        }

        player.DisableGeneralControls();
    }

    public void EnablePlayerInput()
    {
        if (!player)
        {
            Debug.LogError("For some reason, LevelManager can't find player");
            return;
        }

        player.EnableGeneralControls();
    }

    public void DisablePlayerRotation()
    {
        if (!player)
        {
            Debug.LogError("For some reason, LevelManager can't find player");
            return;
        }

        player.DisableRotation();
    }

    public void EnablePlayerRotation()
    {
        if (!player)
        {
            Debug.LogError("For some reason, LevelManager can't find player");
            return;
        }

        player.EnableRotation();
    }

    public Vector4 GetCurrentLargeRoomBounds()
    {
        BossRoomManager b = CurrentScene.manager as BossRoomManager;
        LargeRoomCameraController largeRoom = b.GetLargeRoomCameraController();
        return new Vector4 (largeRoom.rightBound, largeRoom.leftBound, largeRoom.upperBound, largeRoom.lowerBound);
    }
    
    public void SetPlayerSprite(Sprite sprite)
    {
        if (!player) return;

        player.SetSprite(sprite);
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void SetGlobalLightAccordingToWeather()
    {
        if (!globalLight) return;
        if (!enableWeather)
        {
            if (snowPrefab) snowPrefab.SetActive(false);
            globalLight.intensity = 1f;
            return;
        }


        switch (weather)
        {
            case Weather.day_clear:
                globalLight.intensity = dayClearBrightness;
                if (snowPrefab) snowPrefab.SetActive(false);
                if (lightsContainer) lightsContainer.SetActive(false);
                break;
            case Weather.day_snow:
                globalLight.intensity = daySnowBrightness;
                if (snowPrefab) snowPrefab.SetActive(true);
                if (lightsContainer) lightsContainer.SetActive(false);
                break;
            case Weather.night_clear:
                globalLight.intensity = nightClearBrightness;
                if (snowPrefab) snowPrefab.SetActive(false);
                if (lightsContainer) lightsContainer.SetActive(true);
                break;
            case Weather.night_snow:
                if (!isEasyMode) globalLight.intensity = nightSnowBrightness;
                else globalLight.intensity = nightSnowBrightness * 2f;
                if (snowPrefab) snowPrefab.SetActive(true);
                if (lightsContainer) lightsContainer.SetActive(true);
                break;
            default:
                break;
        }
    }

    public bool IsQuitting() => isQuitting;

    public void SetCursorColor(Color newColor)
    {
        color = newColor;
    }

    public void ReturnToMainMenu()
    {
        GameData gm = LoadGameData();

        // save the gun we are holding
        if (gm.weaponID == 4 && gm.foundAndarozGun == false)
        {
            gm.weaponID = 1;
            Save(gm);
        }

        Save(gm);

        isQuitting = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void Save(GameData playerData)
    {
        string data = JsonUtility.ToJson(playerData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/GameData.json", data);
    }

    public GameData LoadGameData()
    {
        string path = Application.persistentDataPath + "/GameData.json";

        if (!System.IO.File.Exists(path))
        {
            Save(new GameData());
        }

        string savedData = System.IO.File.ReadAllText(Application.persistentDataPath + "/GameData.json");

        GameData data = JsonUtility.FromJson<GameData>(savedData);
        
        return data;
    }

    public void ResetLevelProgress()
    {
        GameData fg = LoadGameData();
        fg.beatValkyrie = false;
        fg.firstTimeInHub = true;
        fg.beatGluttony = false;
        fg.beatAndaroz = false;
        Save(fg);
    }

    public void ResetProgressFull()
    {
        GameData fg = LoadGameData();
        fg.beatValkyrie = false;
        fg.firstTimeInHub = true;
        fg.beatGluttony = false;
        fg.beatAndaroz = false;
        fg.foundAndarozGun = false;
        fg.weaponID = 1;
        Save(fg);
    }

    public Weather GetWeather()
    {
        return weather;
    }

    public void ShowCursor()
    {
        if (!cursor || isVisible) return;
        isVisible = true;
        cursor.enabled = true;
    }

    public void HideCursor()
    {
        if (!cursor || !isVisible) return;
        isVisible = false;
        cursor.enabled = false;
    }
    
    public void IDied()
    {
        deadScreen.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(deadScreen.transform.GetChild(1).gameObject);
        StartCoroutine(MusicEnd(200f));
    }

    public void RestartLevel()
    {
        isQuitting = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToHub()
    {
        isQuitting = true;

        // save some settings
        GameData gm = LoadGameData();
        gm.firstTimeInHub = false;
        if (gm.weaponID == 4 && gm.foundAndarozGun == false)
        {
            gm.weaponID = 1;
        }
        Save(gm);

        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }


    //make the music fade out via low pass filter
    public IEnumerator MusicEnd(float min)
    {
        if (backgroundAudioSource)
        {
            AudioLowPassFilter lpf = backgroundAudioSource.GetComponent<AudioLowPassFilter>();
            lpf.enabled = true;
            if (lpf)
            {
                while (lpf.cutoffFrequency > min)
                {
                    lpf.cutoffFrequency = Mathf.Clamp(lpf.cutoffFrequency - onDiedAudioOutSpeed, min, 12000f);
                    yield return null;
                }
            }
            if (min == 10f)
            {
                backgroundAudioSource.mute = true;
            }
        }
    }

    // change songs with a low pass fade transition
    public IEnumerator ChangeSongs(AudioClip next, float startAt = 0f)
    {
        if (backgroundAudioSource)
        {
            AudioLowPassFilter lpf = backgroundAudioSource.GetComponent<AudioLowPassFilter>();
            lpf.enabled = true;
            if (lpf)
            {
                while (lpf.cutoffFrequency > 10)
                {
                    lpf.cutoffFrequency = Mathf.Clamp(lpf.cutoffFrequency - onDiedAudioOutSpeed*2, 10, 12000f);
                    yield return null;
                }

                yield return new WaitForSeconds(0.25f);

                backgroundAudioSource.clip = next;
                backgroundAudioSource.Play();
                backgroundAudioSource.time = startAt;


                while (lpf.cutoffFrequency < 10000)
                {
                    lpf.cutoffFrequency += onDiedAudioOutSpeed;
                    yield return null;
                }
                lpf.enabled = false;
            }
        }
    }

    public void Quit()
    {
        isQuitting = true;
        Application.Quit();
    }
}
