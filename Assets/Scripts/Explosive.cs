using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Explosive : MonoBehaviour
{
    CircleCollider2D blastRadius;
    List<GameObject> victims = new();
    [SerializeField] int damage = 50;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask whatTakesDamage;

    void Start()
    {       
        blastRadius = GetComponent<CircleCollider2D>();
    }

    public void explode()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, blastRadius.radius, whatTakesDamage);
        if (col.Length > 0)
        {

            foreach (var obj in col)
            {
                Debug.Log(obj.gameObject.name);
                StaticHelpers.ApplyDamage(obj.gameObject, damage);
            }
        }

    }
}

