using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AmbushBossManager : BossRoomManager
{
    [Header("Enter Cutscene")]
    public Transform walkToPoint;

    private Animator roomAnimator;
    private Transform playerTransform;
    private Player player;
    public AnimationCurve moveCurve;
    public Transform lightsContainer;
    private Light2D[] roomLights;
    private HPComponent bossHp;
    public string bossName = "THE SCREECHING VALKYRIE";

    const string OPEN_DOORS = "OpenDoors";
    const string CLOSE_DOORS = "CloseDoors";


    [Header("Dialogue")]
    public bool enableDialogue = false;
    public DialogueController.DialoguePart[] doorsClosedScript;
    public DialogueController.DialoguePart[] endScript;

    public AudioSource audioSource;
    public AudioClip bossTheme;

    private void Awake()
    {
        roomAnimator = GetComponent<Animator>();
        roomLights = lightsContainer.GetComponentsInChildren<Light2D>();
    }

    private void Start()
    {
        player = LevelManager.instance.GetPlayer();
        playerTransform = player.transform;
    }

    public override void ForceSetScene()
    {
        base.ForceSetScene();

        LevelManager.instance.DisablePlayerInput();
        CameraShake.instance.restoreCamPosAfterShake = false;

        // switch songs
        StartCoroutine(LevelManager.instance.ChangeSongs(bossTheme, 50f));


        // hide enemies
        for (int i = 0; i < LevelManager.instance.CurrentScene.enemyContainer.transform.childCount; i++)
        {
            LevelManager.instance.CurrentScene.enemyContainer.transform.GetChild(i).gameObject.SetActive(false);
        }

        StartCoroutine(nameof(StartCutscene));

        bossHp = LevelManager.instance.CurrentScene.enemyContainer.GetComponent<HPComponent>();
        bossHp.OnHPZero = EndBossFight;
    }

    private IEnumerator StartCutscene()
    {
        Vector3 start = LevelManager.instance.CurrentScene.playerEntrance.position;
        Vector3 end = Vector3.zero;
        if (walkToPoint) end = walkToPoint.position;
        else end = start + Vector3.right * 5f;

        float startTime = Time.time;
        float totalDist = Vector3.Distance(start, end);

        float alpha = 0;

        // move the player
        while (alpha < 1)
        {
            float distCovered = (Time.time - startTime) * player.runSpeed;
            alpha = moveCurve.Evaluate(distCovered / totalDist);
            playerTransform.position = Vector3.Lerp(start, end, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // close doors
        roomAnimator.Play(CLOSE_DOORS);
        yield return new WaitForSeconds(0.5f);

        // wait for doors to close
        while (roomAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) yield return null;
        CameraShake.instance.ShakeCamera(2f,0.2f);
        
        //dialogue
        if (enableDialogue)
        {
            LevelManager.instance.dialogueController.StartDialogue(doorsClosedScript);
        
            while (!LevelManager.instance.dialogueController.isFinished) yield return null;
        }

        yield return new WaitForSeconds(.5f);

        // shut the lights
        foreach (var item in roomLights)
        {
            if (item.lightType == Light2D.LightType.Point)
            {
                item.enabled = false;
            }
            else if (LevelManager.instance.isEasyMode)
            {
                item.intensity = 0.4f;
            }
            else
            {
                item.intensity = 0.1f;
            }
            yield return new WaitForSeconds(0.1f);
        }

        // start fight
        if (bossBarScript)
        {
            bossBarScript.bossName = bossName;
            bossBarScript.SetLightTheme();
            bossBarScript.gameObject.SetActive(true);
        }

        LevelManager.instance.EnablePlayerInput();

        yield return new WaitForSeconds(0.5f);
        player.FlashlightOn();


        // show enemies
        for (int i = 0; i < LevelManager.instance.CurrentScene.enemyContainer.transform.childCount; i++)
        {
            LevelManager.instance.CurrentScene.enemyContainer.transform.GetChild(i).gameObject.SetActive(true);
        }

        CameraShake.instance.restoreCamPosAfterShake = true;
    }

    private void EndBossFight()
    {
        StartCoroutine(nameof(EndCutscene));
    }

    private IEnumerator EndCutscene()
    {
        StartCoroutine(LevelManager.instance.MusicEnd(3000));

        yield return new WaitForSeconds(0.5f);

        // hide boss bar
        if (bossBarScript)
        {
            bossBarScript.Dissapear();
        }


        yield return new WaitForSeconds(3f);


        // turn on the lights
        foreach (var item in roomLights)
        {
            if (item.lightType == Light2D.LightType.Point)
            {
                item.enabled = true;
            }
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);
        player.FlashlightOff();
        yield return new WaitForSeconds(2f);

        LevelManager.instance.DisablePlayerInput();


        // dialogue
        LevelManager.instance.dialogueController.StartDialogue(endScript);
        while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        yield return new WaitForSeconds(1f);

        GameData gd = LevelManager.instance.LoadGameData();
        gd.beatValkyrie = true;
        gd.firstTimeInHub = false;
        LevelManager.instance.Save(gd);

        LevelManager.instance.NextScene();
    }
}
