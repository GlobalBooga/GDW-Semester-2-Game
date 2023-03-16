using System.Linq;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public enum SceneObjective
    {
        None,
        Boss,
        KillAllEnemies,
        MovementTutorial,
        DodgeTutorial,
        WeaponsTutorial,
        DestructibleObjectsTutorial,
        EnemiesTutorial,
        PickAContract,
        PickAWeapon
    }

    internal const string MOVEMENT_TUTORIAL = "Move";
    internal const string DODGE_TUTORIAL = "Dodge";
    internal const string KILL_ALL_ENEMIES_OBJECTIVE = "Kill All Enemies";
    internal const string PICK_A_CONTRACT_OBJECTIVE = "Pick a contract";
    internal const string WEAPONS_TUTORIAL = "Shoot";
    internal const string DESTRUCTIBLE_OBJECTS_TUTORIAL = "Break stuff";
    internal const string ENEMIES_TUTORIAL = "Observe";
    internal const string PICK_A_WEAPON = "Pick a weapon";



    [Header("Scene Stuff")]
    public SceneObjective[] sceneObjectives;
    public float cameraSize;
    private int enemyCount;
    public bool isIndoors;

    private LevelManager.Scene currentScene;


    public virtual void SetScene()
    {
        if (LevelManager.instance.CurrentScene.skip)
        { 
            LevelManager.instance.NextScene();
            return;
        }

        ForceSetScene();
    }

    public virtual void ForceSetScene()
    {
        currentScene = LevelManager.instance.CurrentScene;

        EnableScene();

        // if no objective
        if (currentScene.exit)
        {
            if (sceneObjectives.Length == 0)
            {
                currentScene.exit.Unblock();
            }
        }

        if (isIndoors) LevelManager.instance.globalLight.intensity = 0f;
        else LevelManager.instance.SetGlobalLightAccordingToWeather();
        Camera.main.transform.position = transform.position + Vector3.back * 10f;
        if (CameraShake.instance) CameraShake.instance.SetCameraSize(cameraSize);

        Transform player = GameObject.Find("Player").transform;
        if (currentScene.playerEntrance)
        {
            player.position = currentScene.playerEntrance.position + (Vector3)((player.position - currentScene.playerEntrance.position) * Vector2.Perpendicular(currentScene.enterDirection));
        }
        else
        {
            player.position = transform.position;
        }


        // enable all enemies and gameplay objects
        if (currentScene.enemyContainer)
        {
            enemyCount = 0;
            foreach (Enemy enemy in currentScene.enemyContainer.GetComponentsInChildren<Enemy>(true))
            {
                enemyCount++;
                enemy.gameObject.SetActive(true);
            }
        }


        // setting the hint message
        if (LevelManager.instance.hintController)
        {


            LevelManager.instance.hintController.ClearObjectives();
            
            
            foreach (var objective in sceneObjectives)
            {
                switch (objective)
                {
                    case SceneObjective.KillAllEnemies:
                        LevelManager.instance.hintController.AddObjective(KILL_ALL_ENEMIES_OBJECTIVE);
                        LevelManager.instance.CurrentScene.exit.Block();
                        break;
                    case SceneObjective.PickAContract:
                        LevelManager.instance.hintController.AddObjective(PICK_A_CONTRACT_OBJECTIVE);
                        if (LevelManager.instance.CurrentScene.exit) Invoke(nameof(Unblock), 0.5f);
                        break;
                    case SceneObjective.PickAWeapon:
                        LevelManager.instance.hintController.AddObjective(PICK_A_WEAPON);
                        if (LevelManager.instance.CurrentScene.exit) Invoke(nameof(Unblock), 0.5f);
                        break;
                    case SceneObjective.MovementTutorial:
                        LevelManager.instance.hintController.AddObjective(MOVEMENT_TUTORIAL);
                        if (LevelManager.instance.CurrentScene.exit) Invoke(nameof(Unblock), 0.5f);
                        break;
                    case SceneObjective.DodgeTutorial:
                        LevelManager.instance.hintController.AddObjective(DODGE_TUTORIAL);
                        if (LevelManager.instance.CurrentScene.exit) Invoke(nameof(Unblock), 0.5f);
                        break;
                    case SceneObjective.WeaponsTutorial:
                        LevelManager.instance.hintController.AddObjective(WEAPONS_TUTORIAL);
                        if (LevelManager.instance.CurrentScene.exit) Invoke(nameof(Unblock), 0.5f);
                        break;
                    case SceneObjective.DestructibleObjectsTutorial:
                        LevelManager.instance.hintController.AddObjective(DESTRUCTIBLE_OBJECTS_TUTORIAL);
                        if (LevelManager.instance.CurrentScene.exit) Invoke(nameof(Unblock), 0.5f);
                        break;
                    case SceneObjective.EnemiesTutorial:
                        LevelManager.instance.hintController.AddObjective(ENEMIES_TUTORIAL);
                        if (LevelManager.instance.CurrentScene.exit) Invoke(nameof(Unblock), 0.5f);
                        break;
                    case SceneObjective.Boss:
                    case SceneObjective.None:
                    default:
                        if (LevelManager.instance.CurrentScene.exit) Invoke(nameof(Unblock), 0.5f);
                        break;
                }
            }

            //if (sceneObjectives.Length > 0 && !LevelManager.instance.hintController.IsShowing) LevelManager.instance.hintController.ShowHint();
        }
    }

    public virtual void DisableScene()
    {
        if (LevelManager.instance.hintController.IsShowing) LevelManager.instance.hintController.HideHint();

        currentScene = LevelManager.instance.CurrentScene;

        if (currentScene.enemyContainer) currentScene.enemyContainer.SetActive(false);
        if (currentScene.gameplayObjectContainer) currentScene.gameplayObjectContainer.SetActive(false);
        if (currentScene.exit) currentScene.exit.enabled = false;
    }

    public virtual void ProgressKillAllEnemies()
    {
        currentScene = LevelManager.instance.CurrentScene;

        foreach (var objective in sceneObjectives)
        {
            if (objective != SceneObjective.KillAllEnemies) return;

            if (--enemyCount <= 0)
            {
                //Debug.Log("scene complete");
                if (currentScene.exit) currentScene.exit.Unblock();
                LevelManager.instance.hintController.ObjectiveComplete(KILL_ALL_ENEMIES_OBJECTIVE);
                return;
            }
        }
    }

    public virtual void ProgressFindTheKey()
    {
        // we dont have this
    }

    public virtual void EnableScene()
    {
        currentScene = LevelManager.instance.CurrentScene;

        if (currentScene.enemyContainer) currentScene.enemyContainer.SetActive(true);
        if (currentScene.gameplayObjectContainer) currentScene.gameplayObjectContainer.SetActive(true);
        if (currentScene.exit) currentScene.exit.enabled = true;
    }

    private void Unblock()
    {
        LevelManager.instance.CurrentScene.exit.Unblock();
    }
}
