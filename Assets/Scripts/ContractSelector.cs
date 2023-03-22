using UnityEngine;

public class ContractSelector : MonoBehaviour
{
    public enum Bosses
    {
        Andaroz,
        Gluttony,
        TimeLord
    }

    public Bosses boss;
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == StaticHelpers.PlayerLayer ||
            collision.gameObject.layer == StaticHelpers.PlayerInvincibleLayer)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.2f) Invoke(nameof(DelayedShowContract), 0.25f);
            else animator.Play("ContractShow");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == StaticHelpers.PlayerLayer ||
            collision.gameObject.layer == StaticHelpers.PlayerInvincibleLayer)
        {
            animator.Play("ContractHide");
        }
    }

    void DelayedShowContract()
    {
        animator.Play("ContractShow");
    }
}
