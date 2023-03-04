using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class HPComponent : MonoBehaviour
{
    public Slider sliderHpBar;
    public List<Image> hpBars;
    public TextMeshProUGUI hpText;
    public float hpColorThreshold = 0.5f;
    public Color fullHpColor = new Color(0.208916f, 0.6792453f, 0.1762193f, 1f);
    public Color midHpColor = new Color(0.735849f, 0.7018685f, 0.1423104f, 1f);
    public Color lowHpColor = new Color(1f, 0f, 0f, 1f);
    public float postDamageInvincibilityTime = 0;
    public bool isInvincible;

    public float maxHealth = 100f;
    private float health;

    public float GetHealth() => health;


    public List<Action> OnHit = new List<Action>();
    public Action OnHPZero;
    public Action OnHalfHP;

    private void Start()
    {
        health = maxHealth;
        foreach (var bar in hpBars)
        {
            bar.color = fullHpColor;
        }

    }

    private void OnDisable()
    {
        //reset
        Start();       
    }

    public void Reduce(float amount)
    {
        if (isInvincible) return;
        if (postDamageInvincibilityTime > 0) isInvincible = true;

        // DEAD
        if ((health -= amount) <= 0f)
        {
            if (OnHPZero != null) OnHPZero.Invoke();

        }

        // CALLING ONHIT
        if (OnHit.Count > 0f)
        {
            foreach (var item in OnHit)
            {
                item.Invoke();
            }
        }

        // UPDATING THE HPBAR(S)
        UpdateBars();

        if (health <= maxHealth / 2 && OnHalfHP != null) OnHalfHP.Invoke();

        if (postDamageInvincibilityTime > 0) Invoke(nameof(ResetDamageable), postDamageInvincibilityTime);
    }

    private void ResetDamageable()
    {
        isInvincible = false;
    }

    public void Add(float amount)
    {
        Mathf.Clamp(health += amount, 0, maxHealth);
        UpdateBars();
    }

    public void UpdateBars()
    {
        if (hpBars.Count > 0)
        {
            float hpPercentage = health / maxHealth;
            foreach (Image bar in hpBars)
            {
                bar.fillAmount = hpPercentage;

                if (hpPercentage <= hpColorThreshold && hpPercentage > hpColorThreshold / 2)
                {
                    //Debug.Log("first half");
                    bar.color = Color.Lerp(midHpColor, fullHpColor, (bar.fillAmount - hpColorThreshold / 2) / (hpColorThreshold / 2));
                }
                else if (hpPercentage <= hpColorThreshold / 2)
                {
                    bar.color = Color.Lerp(lowHpColor, midHpColor, bar.fillAmount / (hpColorThreshold / 2));
                }
            }
        }
        else if (sliderHpBar)
        {
            float hpPercentage = health / maxHealth;
            sliderHpBar.value = hpPercentage;
        }
    }

    public void SetText(string text)
    {
        if (hpText)
        {
            hpText.text = text;
        }
    }
}
