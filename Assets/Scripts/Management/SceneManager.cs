using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public enum SceneObjective
    {
        None,
        Boss,
        KillAllEnemies,
        FindTheKey
    }


    public SceneObjective sceneObjective;
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
            if (sceneObjective == SceneObjective.None)
            {
                currentScene.exit.Unblock();
            }
        }

        if (isIndoors) LevelManager.instance.globalLight.intensity = 0f;
        else LevelManager.instance.SetGlobalLightAccordingToWeather();
        Camera.main.transform.position = transform.position + Vector3.back * 10f;
        CameraShake.instance.SetCameraSize(cameraSize);

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
        switch (sceneObjective)
        {
            case SceneObjective.KillAllEnemies:
                LevelManager.instance.hintText.text = $"Kill All Enemies. {enemyCount} Still Remain!";
                LevelManager.instance.CurrentScene.exit.Block();
                break;
            case SceneObjective.FindTheKey:
                LevelManager.instance.hintText.text = "Find the Key.";
                LevelManager.instance.CurrentScene.exit.Block();
                break;
            case SceneObjective.Boss:
            case SceneObjective.None:
            default:
                Invoke(nameof(Unblock), 0.5f);
                LevelManager.instance.hintText.text = "";
                break;
        }
    }

    public virtual void DisableScene()
    {
        currentScene = LevelManager.instance.CurrentScene;

        if (currentScene.enemyContainer) currentScene.enemyContainer.SetActive(false);
        if (currentScene.gameplayObjectContainer) currentScene.gameplayObjectContainer.SetActive(false);
        if (currentScene.exit) currentScene.exit.enabled = false;
    }

    public virtual void ProgressKillAllEnemies()
    {
        currentScene = LevelManager.instance.CurrentScene;

        if (sceneObjective != SceneObjective.KillAllEnemies) return;

        if (--enemyCount <= 0)
        {
            //Debug.Log("scene complete");
            if (currentScene.exit) currentScene.exit.Unblock();
            LevelManager.instance.hintText.text = "";
            return;
        }

        LevelManager.instance.hintText.text = $"Kill All Enemies. {enemyCount} Still Remain!";
    }

    public virtual void ProgressFindTheKey()
    {
        if (sceneObjective != SceneObjective.FindTheKey) return;
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
