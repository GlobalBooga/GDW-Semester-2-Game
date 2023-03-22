using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] Object destructableObj;
    [SerializeField] bool hasPieces;

    public HPComponent hp;
    public bool respawn;
    public float respawnDelay;

    private bool respawning;

    GameObject destructable;

    SpriteRenderer sr;
    Collider2D coll2d;

    void Start()
    {
        hp = GetComponent<HPComponent>();
        if (hp) hp.OnHPZero = BreakObject;
        sr = GetComponent<SpriteRenderer>();
        coll2d = GetComponent<Collider2D>();
    }

    private void BreakObject()
    {
        if (hasPieces == true)
        {
            destructable = (GameObject)Instantiate(destructableObj);
            destructable.transform.position = transform.position;
            if (respawn) Destroy(destructable, respawnDelay);
        }

        if (respawn)
        {
            if (!respawning)
            {
                respawning = true;
                coll2d.enabled = false;
                if (destructableObj) sr.color = Color.clear; 
                else sr.color = new Color(1,1,1,0.3f);
                Invoke(nameof(Respawn), respawnDelay);
            
                if (transform.childCount > 0)
                {
                    Explosive exp = transform.GetChild(0).GetComponent<Explosive>();
                    if (exp)
                    {
                        exp.Explode();
                    }
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Respawn()
    {
        respawning = false;
        coll2d.enabled = true;
        sr.color = Color.white;
        hp.Add(hp.maxHealth);
    }
}
