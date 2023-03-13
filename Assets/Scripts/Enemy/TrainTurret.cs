using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTurret : TrainWeapon
{
    internal override void Hit()
    {
        anim.Play($"TurretHit{hitIndex}", 5+hitIndex);
    }

}
