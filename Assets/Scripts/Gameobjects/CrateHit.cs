using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateHit : MonoBehaviour
{
    Animator animator;
    HPComponent hp;

    void Start()
    {
        animator = GetComponent<Animator>();
        hp = GetComponent<HPComponent>();
        if (hp)
        {
            hp.OnHit.Add(OnHit);
        }
    }

    void OnHit()
    {
        if (animator.isActiveAndEnabled) animator.Play("CrateHit");
    }

}
