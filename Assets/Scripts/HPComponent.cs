using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class HPComponent : MonoBehaviour
{
    public Image hpBar;
    public TextMeshProUGUI hpText;
    public Color fullHpColor = new Color(0.208916f, 0.6792453f, 0.1762193f, 1f);
    public Color midHpColor = new Color(0.735849f, 0.7018685f, 0.1423104f, 1f);
    public Color lowHpColor = new Color(0.5943396f, 0.1654058f, 0.1654058f, 1f);

    public bool isInvincible;

    public float maxHealth;
    float health;

    public List<Action> OnHit = new List<Action>();
    public Action OnHPZero;

    public void Reduce(float amount)
    {
        if (isInvincible) return;

        // DEAD
        if ((health -= amount) <= 0f)
        {
            OnHPZero.Invoke();

        }
        UpdateHud();


        // CALLING ONHIT
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


    public void SetHPBar(float hpPercentage)
    {
        hpBar.fillAmount = hpPercentage;

        // Dead
        if (hpPercentage <= 0f && OnHPZero != null)
        {
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
