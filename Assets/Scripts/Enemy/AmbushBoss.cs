using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushBoss : MonoBehaviour
{
    HPComponent hp;
    private List<HPComponent> allHpComps = new();

    // Start is called before the first frame update
    void Start()
    {
        hp = GetComponent<HPComponent>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            allHpComps.Add(transform.GetChild(i).GetComponentInChildren<HPComponent>(true));
            allHpComps[i].OnHit.Add(UpdateBossHP);
            hp.maxHealth += allHpComps[i].maxHealth;
        }
    }

    private void UpdateBossHP()
    {
        float health = 0f;
        foreach (var item in allHpComps)
        {
            health += item.GetHealth();
        }
        hp.Reduce(hp.GetHealth() - health);
    }
}
