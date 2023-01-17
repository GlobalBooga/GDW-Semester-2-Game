using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyScriptableObject enemyScriptable;
    SpriteRenderer sr;
    

    public enum EnemyType
    {
        Melee,
        Flying
    }

    public EnemyType enemyType;

    private void OnValidate()
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                if (!gameObject.GetComponent<GroundedNPCMovement>())
                {
                    gameObject.AddComponent<GroundedNPCMovement>();
                }
                break;
            case EnemyType.Flying:
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        if (enemyScriptable != null)
        {
            enemyScriptable.TakeDamage(amount);
        }
    }
}
