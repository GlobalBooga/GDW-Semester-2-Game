using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomManager : SceneManager
{
    [SerializeField] internal BossBar bossBarScript;
    [SerializeField] internal LargeRoomCameraController largeRoomCamera;

    private void Update()
    {
        if (LevelManager.instance.startBossBattle)
        {
            LevelManager.instance.startBossBattle = false;
            if (bossBarScript)
            {
                bossBarScript.SetDarkTheme();
                bossBarScript.bossName = LevelManager.instance.bossTitle;
                bossBarScript.gameObject.SetActive(true);
            }
            if (largeRoomCamera) largeRoomCamera.enabled = true;
        }
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


    public LargeRoomCameraController GetLargeRoomCameraController()
    {
        return largeRoomCamera;
    }
}
