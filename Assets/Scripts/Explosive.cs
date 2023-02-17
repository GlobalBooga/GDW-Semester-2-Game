using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Explosive : MonoBehaviour
{
    CircleCollider2D blastRadius;
    [SerializeField] int damage = 50;
    [SerializeField] GameObject explosionObj;
    [SerializeField] LayerMask whatTakesDamage;

    void Start()
    {       
        blastRadius = GetComponent<CircleCollider2D>();
        blastRadius.enabled = false;
    }

    private void OnDestroy()
    {
        Explode();
    }

    public void Explode()
    {
        GameObject g = Instantiate(explosionObj, transform);
        g.transform.parent = null;
        Destroy(g, 1f);

        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, blastRadius.radius, whatTakesDamage);
        if (col.Length > 0)
        {

            foreach (var obj in col)
            {
                StaticHelpers.ApplyDamage(obj.gameObject, damage);
            }
        }

    }
}

