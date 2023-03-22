using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainBossManager : BossRoomManager
{
    [Header("Enter Cutscene")]
    public Transform cutsceneFocusPoint;
    public Transform enemiesGoToPoint;
    public TrainBoss boss;

    private Animator roomAnimator;
    private Player player;
    private Transform playerTransform;

    public bool skipCutscene;
    public bool enableDialogue = true;

    [Header("Dialogue")]
    public DialogueController.DialoguePart[] enterScript;
    public DialogueController.DialoguePart[] noticedScript;
    public DialogueController.DialoguePart[] fightStartScript;
    public DialogueController.DialoguePart[] endScript;

    private bool startboss;


    private void Start()
    {
        roomAnimator = GetComponent<Animator>();
        player = LevelManager.instance.GetPlayer();
        playerTransform = player.transform;
    }

    public override void ForceSetScene()
    {
        base.ForceSetScene();

        largeRoomCamera.enabled = true;
        if (skipCutscene) return;

        LevelManager.instance.DisablePlayerInput();
        CameraShake.instance.restoreCamPosAfterShake = false;


        if (boss) boss.enabled = false;

        StartCoroutine(nameof(StartCutscene));
    }

    private IEnumerator StartCutscene()
    {
        yield return new WaitForSeconds(0.5f);

        if (enableDialogue)
        {
            // dialogue
            LevelManager.instance.dialogueController.StartDialogue(enterScript);
            while (!LevelManager.instance.dialogueController.isFinished) yield return null;

            yield return new WaitForSeconds(0.2f);
        }

        LevelManager.instance.EnablePlayerInput();
        CameraShake.instance.restoreCamPosAfterShake = true;


        // wait for player to get noticed
        while (!LevelManager.instance.playerFound && !startboss)
        {
            yield return null;
        }

        foreach (var item in LevelManager.instance.CurrentScene.enemyContainer.transform.GetComponentsInChildren<Enemy>())
        {
            item.enabled = false;
        }

        LevelManager.instance.DisablePlayerInput();
        CameraShake.instance.restoreCamPosAfterShake = false;

        if (cutsceneFocusPoint)
        {
            largeRoomCamera.enabled = false;
            CameraShake.instance.SetCameraFollow(cutsceneFocusPoint);
            if (Camera.main.orthographicSize > 15) CameraShake.instance.LerpCameraSize(15f, 1);
        }


        while (Camera.main.orthographicSize > 15)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        if (enableDialogue)
        {
            LevelManager.instance.dialogueController.StartDialogue(noticedScript);
            while (!LevelManager.instance.dialogueController.isFinished) yield return null;
        
            yield return new WaitForSeconds(0.2f);
        }

        // enemies run in the train

        foreach (var item in LevelManager.instance.CurrentScene.enemyContainer.transform.GetComponentsInChildren<Enemy>())
        {
            item.enabled = true;
            item.fov = 0f;
        }
        LevelManager.instance.AlertAllEnemiesInCurrentScene(enemiesGoToPoint.position);

        yield return new WaitForSeconds(0.3f);
        roomAnimator.Play("TrainBossStart");


        yield return new WaitForSeconds(3f);


        if (cutsceneFocusPoint)
        {
            CameraShake.instance.SetCameraFollow(Camera.main.transform);
            CameraShake.instance.RestoreCamPos();
            largeRoomCamera.enabled = true;
            //CameraShake.instance.LerpCameraSize(28f, 3f);
        }
        
        yield return new WaitForSeconds(3f);

        if (enableDialogue)
        {
            LevelManager.instance.dialogueController.StartDialogue(fightStartScript);
            while (!LevelManager.instance.dialogueController.isFinished) yield return null;
        }

        // start
        if (bossBarScript)
        {
            bossBarScript.bossName = LevelManager.instance.bossTitle;
            if (LevelManager.instance.enableWeather)
            {
                if (LevelManager.instance.globalLight.intensity > 0.6f)
                { 
                    bossBarScript.SetDarkTheme();

                }
                else
                {
                    bossBarScript.SetLightTheme();
                }
            }
            else
            {
                bossBarScript.SetDarkTheme();
            }
            bossBarScript.gameObject.SetActive(true);
        }

        for (int i = LevelManager.instance.CurrentScene.enemyContainer.transform.childCount -1; i > 0; i--)
        {
            Transform child = LevelManager.instance.CurrentScene.enemyContainer.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        LevelManager.instance.EnablePlayerInput();
        CameraShake.instance.restoreCamPosAfterShake = true;

        boss.enabled = true;
    }


    public IEnumerator EndCutscene()
    {
        yield return new WaitForSeconds(5f);

        LevelManager.instance.DisablePlayerInput();

        if (enableDialogue)
        {
            // dialogue
            LevelManager.instance.dialogueController.StartDialogue(endScript);
            while (!LevelManager.instance.dialogueController.isFinished) yield return null;

            yield return new WaitForSeconds(1.5f);
        }

        GameData gd = LevelManager.instance.LoadGameData();
        gd.beatGluttony = true;
        LevelManager.instance.Save(gd);

        LevelManager.instance.NextScene();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            LevelManager.instance.AlertAllEnemiesInCurrentScene(playerTransform.position);
            startboss = true;
        }
    }
}
