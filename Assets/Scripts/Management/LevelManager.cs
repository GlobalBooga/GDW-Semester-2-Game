using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
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

    [Header("General Scene Settings")]
    public AnimationCurve messageFade;
    public float hintTime = 5f;
    public Text hintText;
    public bool loop;

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

    private void Awake()
    {
        instance = this;
        
    }

    private void Start()
    {
        // disable all scenes besides the first one
        for (int i = 0; i < orderedScenes.Count; i++)
        {
            currentSceneIndex = i;
            orderedScenes[i].manager.DisableScene();
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
}
