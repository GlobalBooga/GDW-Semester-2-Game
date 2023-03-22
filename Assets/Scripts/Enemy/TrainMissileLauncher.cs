using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMissileLauncher : TrainWeapon
{
    internal override void Hit()
    {
        anim.Play($"MissileLauncherHit{hitIndex}", 8 + hitIndex);
    }
}
