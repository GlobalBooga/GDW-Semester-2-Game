using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireZone : MonoBehaviour
{
    private float time;
    private float damageInterval;
    private float damage;
    private TrainBossScriptableObject tso;

    private void OnTriggerStay2D(Collider2D collision)
    {
        time += Time.deltaTime;
        if (time >= damageInterval)
        {
            time = 0;
            Debug.Log(collision.transform.gameObject);
            StaticHelpers.ApplyDamage(collision.transform.gameObject, damage);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        time = 0;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    public void SetDamageInterval(float dmgInt)
    {
        damageInterval = dmgInt;
    }
}
