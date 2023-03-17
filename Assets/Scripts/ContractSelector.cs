using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractSelector : MonoBehaviour
{
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.2f) Invoke(nameof(DelayedShowContract), 0.25f);
        else animator.Play("ContractShow");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        animator.Play("ContractHide");
    }

    void DelayedShowContract()
    {
        animator.Play("ContractShow");
    }
}
