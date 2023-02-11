using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticHelpers 
{
    public static LayerMask PlayerLayer => LayerMask.NameToLayer("Player");
    public static LayerMask EnemyLayer => LayerMask.NameToLayer("Enemy");
    public static LayerMask WallLayer => LayerMask.NameToLayer("Wall");
    public static LayerMask EnemyProjectileLayer => LayerMask.NameToLayer("EnemyProjectile");
    public static LayerMask PlayerProjectileLayer => LayerMask.NameToLayer("PlayerProjectile");
    public static LayerMask PickupLayer => LayerMask.NameToLayer("Pickup");
}
