using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainFlamethrower : TrainWeapon
{
    internal override void Hit()
    {
        anim.Play($"FlamethrowerHit{hitIndex}", 3 + hitIndex);
    }

}
