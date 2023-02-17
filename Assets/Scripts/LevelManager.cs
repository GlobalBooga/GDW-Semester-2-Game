using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public bool doesLevelLoop = false;
    private static bool loop;
    public List<SceneSettings> orderedScenes;
    private static List<SceneSettings> scenes;
    private static Enemy[] allEnemies;
    private static int currentSceneIndex = 0;
    
    private void Start()
    {
        scenes = orderedScenes;
        loop = doesLevelLoop;

        // disable all scenes besides the first one
        for (int i = 0; i < orderedScenes.Count; i++)
        {
            if (i > 0)
            {
                orderedScenes[i].DisableScene();
                continue;
            }

            SceneSettings ss = scenes[currentSceneIndex];
            ss.SetScene();
            if (ss.EnemyContainer)
                allEnemies = ss.EnemyContainer.GetComponentsInChildren<Enemy>();
            else allEnemies = null;
        }
    }

    public static SceneSettings GetCurrentScene()
    {
        return scenes[currentSceneIndex];
    }

    public static void AlertAllEnemiesInCurrentScene(Vector3 alertOrigin)
    {
        if (allEnemies == null) return;

        if (allEnemies.Length > 0)
        {
            foreach (Enemy enemy in allEnemies)
            {
                enemy.Alert(alertOrigin);
            }
        }
    }

    public static void NextScene()
    {
        IncrementScene(1);   
    }

    private static void IncrementScene(int i)
    {
        // close the current scene
        scenes[currentSceneIndex].DisableScene();


        // if last scene in level and not a looping level
        if (!loop && currentSceneIndex == scenes.Count - 1)
        {
            // end level
            Debug.Log("end level");

            // do something else

            return;
        }

        currentSceneIndex = ((currentSceneIndex += i) < scenes.Count) ? currentSceneIndex : 0;

        SceneSettings ss = scenes[currentSceneIndex];
        ss.SetScene();
        if (ss.EnemyContainer)
            allEnemies = ss.EnemyContainer.GetComponentsInChildren<Enemy>();
        else allEnemies = null;
    }

    public static void PreviousScene()
    {
        IncrementScene(-1);
    }


    public static void EnemyDied()
    {
        scenes[currentSceneIndex].ProgressKillAllEnemies();
    }

    public static void KeyCollected()
    {
        scenes[currentSceneIndex].ProgressFindTheKey();
    }

    public static void ShowHint()
    {
        scenes[currentSceneIndex].ShowHint();
    }

    public static void RotateControls(float angle)
    {
        GameObject.Find("Player").GetComponent<Player>().rotateControlsbyAngle = angle;
    }
}
