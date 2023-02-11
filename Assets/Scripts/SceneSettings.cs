using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSettings : MonoBehaviour
{
    public enum SceneObjective
    {
        None,
        KillAllEnemies,
        FindTheKey
    }


    public Transform playerEntrance;
    public Vector2 sceneForward;
    public SceneExit sceneExit;
    public float cameraSize;
    public GameObject EnemyContainer;
    public GameObject gameplayObjectsContainer;
    public SceneObjective sceneObjective;
    private int enemyCount;


    public void SetScene()
    {
        EnableScene();

        if (sceneExit)
        {
            if (sceneObjective == SceneObjective.None)
            {
                sceneExit.Unblock();
            }
        }

        Camera.main.transform.position = transform.position + Vector3.back * 10f;
        Camera.main.orthographicSize = cameraSize;

        Transform player = GameObject.Find("Player").transform;
        if (playerEntrance)
        {
            player.position = playerEntrance.position;
        }
        else
        {
            player.position += (Vector3)sceneForward;
        }

        // enable all enemies and gameplay objects

        if (EnemyContainer)
        {
            enemyCount = 0;
            foreach (Enemy enemy in EnemyContainer.GetComponentsInChildren<Enemy>(true))
            {
                enemyCount++;
                enemy.gameObject.SetActive(true);
            }
        }
    }

    public void ProgressKillAllEnemies()
    {
        if (sceneObjective != SceneObjective.KillAllEnemies) return;

        if (--enemyCount <= 0)
        {
            Debug.Log("scene complete");
            if (sceneExit) sceneExit.Unblock();
        }
    }

    public void ProgressFindTheKey()
    {
        if (sceneObjective != SceneObjective.FindTheKey) return;
    }

    public void DisableScene()
    {
        if (EnemyContainer) EnemyContainer.SetActive(false);
        if (gameplayObjectsContainer) gameplayObjectsContainer.SetActive(false);
    }

    public void EnableScene()
    {
        if (EnemyContainer) EnemyContainer.SetActive(true);
        if (gameplayObjectsContainer) gameplayObjectsContainer.SetActive(true);
    }
}
