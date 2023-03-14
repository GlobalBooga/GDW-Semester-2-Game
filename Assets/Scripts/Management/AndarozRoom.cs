using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndarozRoom : BossRoomManager
{

    public override void ForceSetScene()
    {
        base.ForceSetScene();

        largeRoomCamera.enabled = true;

        LevelManager.instance.DisablePlayerInput();
        CameraShake.instance.restoreCamPosAfterShake = false;
        CameraShake.instance.isLargePanningRoom = true;
    }
}
