using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 100f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == StaticHelpers.PlayerLayer)
        {
            HPComponent hp;
            if (collision.gameObject.TryGetComponent(out hp))
            {
                hp.Add(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
