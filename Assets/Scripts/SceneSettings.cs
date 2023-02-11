using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI hintText;
    public float hintTime = 5f;
    private bool showingHint;
    public AnimationCurve messageFade;

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


        // setting the hint message
        switch (sceneObjective)
        {
            case SceneObjective.None:
                hintText.text = "";
                break;
            case SceneObjective.KillAllEnemies:
                hintText.text = $"Kill All Enemies. {enemyCount} Still Remain!";
                break;
            case SceneObjective.FindTheKey:
                hintText.text = "Find the Key.";
                break;
            default:
                break;
        }
    }

    public void ProgressKillAllEnemies()
    {
        if (sceneObjective != SceneObjective.KillAllEnemies) return;

        if (--enemyCount <= 0)
        {
            //Debug.Log("scene complete");
            if (sceneExit) sceneExit.Unblock();
        }

        hintText.text = $"Kill All Enemies. {enemyCount} Still Remain!";
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

    public void ShowHint()
    {
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
            hintText.color = Color.Lerp(Color.white,Color.clear, messageFade.Evaluate(time));
            yield return null;
        }
        hintText.enabled = false;
    }
}
