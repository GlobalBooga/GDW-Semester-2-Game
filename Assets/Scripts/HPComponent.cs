using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class HPComponent : MonoBehaviour
{
    public Image hpBar;
    public TextMeshProUGUI hpText;
    public Color fullHpColor;
    public Color midHpColor;
    public Color lowHpColor;

    public bool isInvincible;

    public float maxHealth;
    float health;

    public List<Action> OnHit = new List<Action>();
    public Action OnHPZero;

    public void Reduce(float amount)
    {
        if (isInvincible) return;

        if ((health -= amount) <= 0f)
        {
            gameObject.SetActive(false);
        }
        UpdateHud();


        // calling all onhits
        if (OnHit.Count > 0f)
        {
            foreach (var item in OnHit)
            {
                item.Invoke();
            }
        }

    }

    public void Add(float amount)
    {
        Mathf.Clamp(health += amount, 0, maxHealth);
        UpdateHud();
    }

    /*private void OnEnable()
    {
        health = maxHealth;
        UpdateHud();
        if (hud)
        {
            hud.ResetHud();
        }
    }*/

    void UpdateHud()
    {
        /*if (hud)
        {
            hud.SetHP(health / maxHealth);
            hud.SetText(health.ToString());
        }*/
    }


    public void SetHP(float hpPercentage)
    {
        hpBar.fillAmount = hpPercentage;

        // Dead
        if (hpPercentage <= 0f && OnHPZero != null)
        {
            OnHPZero.Invoke();
        }

        /*// healthy hp color
        else if (hpPercentage <= hpColorThreshold && hpPercentage > hpColorThreshold / 2)
        {
            hpBar.color = Color.Lerp(midHpColor, fullHpColor, (hpBar.fillAmount - hpColorThreshold / 2) / (hpColorThreshold / 2));
        }

        // low hp color
        else if (hpPercentage <= hpColorThreshold / 2)
        {
            hpBar.color = Color.Lerp(lowHpColor, midHpColor, hpBar.fillAmount / (hpColorThreshold / 2));
        }*/
    }

    public void SetText(string text)
    {
        if (hpText)
        {
            hpText.text = text;
        }
    }
}
