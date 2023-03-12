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

        while (alpha < 1)
        {
            float distCovered = (Time.time - startTime) * player.runSpeed;
            alpha = moveCurve.Evaluate(distCovered / totalDist);
            playerTransform.position = Vector3.Lerp(start, end, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1f);


        // dialogue

        yield return new WaitForSeconds(0.3f);
        Debug.Log("Adana: \"oh shit dead end\"");
        yield return new WaitForSeconds(2f);


        // close doors

        roomAnimator.Play(CLOSE_DOORS);

        yield return new WaitForSeconds(0.7f);
        Debug.Log("Room: \"its a trap lol get fucked\"");

        // wait for doors to close
        while (roomAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) yield return null;

        CameraShake.instance.ShakeCamera(2f,0.2f);


        yield return new WaitForSeconds(.5f);
        

        // shut the lights
        foreach (var item in roomLights)
        {
            if (item.lightType == Light2D.LightType.Point)
            {
                item.enabled = false;
            }
            yield return new WaitForSeconds(0.1f);
        }


        // start
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
        //yield return new WaitForSeconds(0.5f);


        //LevelManager.instance.DisablePlayerInput();


        // dialogue


        // teleport
    }
}
