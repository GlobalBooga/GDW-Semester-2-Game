using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public static Hud instance;
    public Image weaponIcon;
    public Image abilityIcon;
    public Image abilityCooldown;

    private void Awake()
    {
        instance = this;
    }

    public void SetWeaponIconImage(Sprite sprite)
    {
        if (!weaponIcon) return;
        weaponIcon.sprite = sprite;
    }

    public void SetAbilityIconImage(Sprite sprite)
    {
        if (!abilityIcon) return;
        abilityIcon.sprite = sprite;
    }
}
