using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public Animator animator;
    public Text textObj;
    public Color lightTheme;
    public Color darkTheme;
    [HideInInspector] public string bossName;

    private const string APPEAR = "BossHpBarAppear";
    private const string DISSAPEAR = "BossHpBarGone";

    private void OnEnable()
    {
        if (animator)
        {
            animator.Play(APPEAR);
            Invoke(nameof(DisableAnimator), 2.1f);
        }
        if (textObj) textObj.text = bossName;
    }

    private void Dissapear()
    {
        animator.Play(DISSAPEAR);
        Invoke(nameof(DisableThis), 0.1f);
    }

    private void DisableThis()
    {
        gameObject.SetActive(false);
    }

    private void DisableAnimator()
    {
        animator.enabled = false;
    }

    public void SetLightTheme()
    {
        textObj.color = lightTheme;
    }

    public void SetDarkTheme()
    {
        textObj.color = darkTheme;
    }
}
