using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireZone : MonoBehaviour
{
    private float time;
    private float damageInterval;
    private float damage;
    private TrainBossScriptableObject tso;

    private void Start()
    {
        Transform p = transform;
        TrainBoss tb = null;
        while (!tb)
        {
            p = p.parent;
            tb = p.GetComponent<TrainBoss>();
            //Debug.Log(p.gameObject.name);
        }

        tso = tb.tso;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        damage = tso.flamethrower_damage;
        damageInterval = tso.flamethrower_dmgInterval;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        time += Time.deltaTime;
        if (time >= damageInterval)
        {
            time = 0;
            StaticHelpers.ApplyDamage(collision.gameObject, damage);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        time = 0;
    }
}
