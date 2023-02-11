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
    public static LayerMask EnemyMissile => LayerMask.NameToLayer("EnemyMissile");
    public static LayerMask PlayerInvincibleLayer => LayerMask.NameToLayer("PlayerInvincible");

    public static bool ApplyDamage(GameObject other, float damage)
    {
        HPComponent hp;
        if (other.TryGetComponent(out hp))
        {
            hp.Reduce(damage);
            return true;
        }
        return false;
    }
}
