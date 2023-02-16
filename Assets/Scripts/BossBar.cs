using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public Animator animator;
    public Text textObj;
    [HideInInspector] public string bossName;

    private const string APPEAR = "BossHpBarAppear";
    private const string DISSAPEAR = "BossHpBarGone";

    private void OnEnable()
    {
        if (animator) animator.Play(APPEAR);
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
}
