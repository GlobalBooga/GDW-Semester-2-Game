using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomManager : SceneManager
{
    [SerializeField] private BossBar bossBarScript;
    [SerializeField] private LargeRoomCameraController largeRoomCamera;

    public override void SetScene()
    {
        base.SetScene();
    }

    public override void ForceSetScene()
    {
        base.ForceSetScene();

        if (bossBarScript)
        {
            bossBarScript.bossName = LevelManager.instance.bossTitle;

            bossBarScript.gameObject.SetActive(true);
        }
        if (largeRoomCamera) largeRoomCamera.enabled = true;

        CameraShake.instance.restoreCamPosAfterShake = false;
    }

    public override void DisableScene()
    {
        base.DisableScene();

        if (bossBarScript)
        {
            bossBarScript.gameObject.SetActive(false);
        }
        if (largeRoomCamera) largeRoomCamera.enabled = false;

        CameraShake.instance.restoreCamPosAfterShake = true;
    }

    public override void EnableScene()
    {
        base.EnableScene();
    }

    public override void ProgressKillAllEnemies()
    {
        base.ProgressKillAllEnemies();
    }
}
