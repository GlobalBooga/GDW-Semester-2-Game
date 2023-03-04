using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Explosive : MonoBehaviour
{
    CircleCollider2D blastRadius;
    [SerializeField] int damage = 50;
    [SerializeField] GameObject explosionObj;
    [SerializeField] LayerMask whatTakesDamage;

    private bool isQuitting;

    void Start()
    {       
        blastRadius = GetComponent<CircleCollider2D>();
        blastRadius.enabled = false;
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;   
    }

    private void OnDestroy()
    {
        if (!isQuitting) Explode();
    }

    private void Explode()
    {
        GameObject g = Instantiate(explosionObj, transform);
        g.transform.parent = null;
        Destroy(g, 1f);

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, blastRadius.radius, whatTakesDamage);
        if (cols.Length > 0)
        {
            foreach (var obj in cols)
            {
                if (obj.gameObject.layer == StaticHelpers.SpecialBreakableObjectLayer)
                    StaticHelpers.ApplyDamage(obj.gameObject, damage * 0.25f);
                else if (obj.transform.parent)
                    StaticHelpers.ApplyDamage(obj.transform.parent.gameObject, damage);
                else
                    StaticHelpers.ApplyDamage(obj.gameObject, damage);
            }
        }
    }
}

