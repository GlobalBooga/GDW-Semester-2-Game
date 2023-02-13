using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [SerializeField] Image hpBar;
    [SerializeField] Animator HitIndicator;
    [SerializeField] GameObject deadScreen;
    [SerializeField, Range(0.1f,0.9f)] float hpColorThreshold = 0.5f;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Color fullHpColor;
    [SerializeField] Color midHpColor;
    [SerializeField] Color lowHpColor;

    private void Awake()
    {
        if (deadScreen) deadScreen.SetActive(false);
        hpBar.color = fullHpColor;
    }

    public void ResetHud()
    {
        if (!deadScreen) return;
        deadScreen.SetActive(false);
        hpBar.color = fullHpColor;
    }

    public void SetHP(float hpPercentage)
    {
        if (hpPercentage < hpBar.fillAmount && HitIndicator) HitIndicator.Play("HitFeedback");
        hpBar.fillAmount = hpPercentage;
        if (hpPercentage <= 0f && deadScreen)
        {
            deadScreen.SetActive(true);
        }
        else if (hpPercentage <= hpColorThreshold && hpPercentage > hpColorThreshold/2)
        {
            hpBar.color = Color.Lerp(midHpColor, fullHpColor, (hpBar.fillAmount - hpColorThreshold/2) / (hpColorThreshold/2));
        }
        else if (hpPercentage <= hpColorThreshold/2)
        {
            hpBar.color = Color.Lerp(lowHpColor, midHpColor, hpBar.fillAmount / (hpColorThreshold/2));
        }
    }

    public void SetText(string text)
    {
        if (this.text)
        {
            this.text.text = text;
        }
    }
}
