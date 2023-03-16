using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Crosshair Settings")]
    public Color color;
    //public Sprite cursorSprite;
    public Image cursor;
    //private SpriteRenderer cursorSpriteRenderer;

    [Header("Dialogue")]
    public DialogueController dialogueController;


    [Header("General Scene Settings")]
    public AnimationCurve messageFade;
    public float hintTime = 5f;
    public Text hintText;
    public bool loop;

    [Header("Weather Settings")]
    public bool enableWeather;
    public float dayClearBrightness = 1f;
    public float daySnowBrightness = 0.8f;
    public float nightClearBrightness = 0.3f;
    public float nightSnowBrightness = 0.1f;
    public Light2D globalLight;
    public GameObject snowPrefab;
    private Weather weather;

    [Header("Scene Transition")]
    public Animator screenOverlayAnimator;
    public float transitionTime = 0.2f;
    private const string TRANSITION_ANIM = "SceneTransition";

    [Header("Boss")]
    public string bossTitle = "ANDAROZ THE GREEDY";

    [Header("Scenes")]
    [SerializeField] private List<Scene> orderedScenes;
    public Scene CurrentScene => orderedScenes[currentSceneIndex];

    private int currentSceneIndex = 0;
    private bool showingHint;
    private bool isQuitting;
    public bool playerFound { get; set; }

    private Player player;





    [HideInInspector] public bool startBossBattle;

    private void Awake()
    {
        instance = this;
        player = GameObject.Find("Player").GetComponent<Player>();
        //if (cursor) cursorSpriteRenderer = cursorObject.GetComponent<SpriteRenderer>();

        weather = (Weather)UnityEngine.Random.Range(0, (int)Weather.max);
        SetGlobalLightAccordingToWeather();
    }

    private void Update()
    {
        if (cursor)
        {
            cursor.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            cursor.color = color;
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void Start()
    {
        Cursor.visible = false;

        // disable all scenes besides the first one
        for (int i = 0; i < orderedScenes.Count; i++)
        {
            currentSceneIndex = i;
            orderedScenes[i].manager.DisableScene();
        }

        orderedScenes[currentSceneIndex = 0].manager.SetScene();

        if (!globalLight)
        {
            enableWeather = false;
        }
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
        if (currentSceneIndex == orderedScenes.Count - 1 && !loop)
        {
            // end level
            Debug.Log("end level. Return to hub");

            // Return to hub

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

    public void KeyCollected()
    {
        orderedScenes[currentSceneIndex].manager.ProgressFindTheKey();
    }

    public void ShowHint()
    {
        if (!hintText) return;

        hintText.enabled = true;
        hintText.color = new Color(1, 1, 1, 1);

        if (showingHint) StopCoroutine(nameof(HideHint));

        StartCoroutine(nameof(HideHint));
    }

    private IEnumerator HideHint()
    {
        showingHint = true;
        yield return new WaitForSeconds(hintTime);

        // make the hint fade away
        float time = 0;
        while (messageFade.Evaluate(time) < 1)
        {
            time += Time.deltaTime;
            hintText.color = Color.Lerp(Color.white, Color.clear, messageFade.Evaluate(time));
            yield return null;
        }
        hintText.enabled = false;
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
            globalLight.intensity = 1f;
            return;
        }

        Debug.Log(weather);
        switch (weather)
        {
            case Weather.day_clear:
                globalLight.intensity = dayClearBrightness;
                if (snowPrefab) snowPrefab.SetActive(false);
                break;
            case Weather.day_snow:
                globalLight.intensity = daySnowBrightness;
                if (snowPrefab) snowPrefab.SetActive(true);
                break;
            case Weather.night_clear:
                globalLight.intensity = nightClearBrightness;
                if (snowPrefab) snowPrefab.SetActive(false);
                break;
            case Weather.night_snow:
                globalLight.intensity = nightSnowBrightness;
                if (snowPrefab) snowPrefab.SetActive(true);
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

}
