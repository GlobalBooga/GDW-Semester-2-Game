using UnityEngine;
using UnityEngine.UI;

public class StaminaRecharge : MonoBehaviour
{
    public Color rechargeColor = Color.grey;
    public Slider slider;
    public Image image;

    public void ValueChanging()
    {
        if (!slider && !image) return;

        if (slider.value < 1) image.color = rechargeColor;
        else image.color = Color.white;
    }
}
