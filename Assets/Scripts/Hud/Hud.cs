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

    public void ShowDirectionToNextPart()
    {
        DirectionArrows da = GetComponent<DirectionArrows>();
        if (!da) return;

        // vector from center of scene to exit
        Vector2 dir = LevelManager.instance.CurrentScene.exit.transform.position - LevelManager.instance.CurrentScene.manager.gameObject.transform.position;

        // is it pointing up?
        if (dir.y > dir.x && dir.y > 2) GetComponent<DirectionArrows>().ShowNextSceneDirection(Vector2.up);
        else if (dir.y < dir.x && dir.y < -2) GetComponent<DirectionArrows>().ShowNextSceneDirection(Vector2.down);
        else if (dir.x > dir.y && dir.x > 2) GetComponent<DirectionArrows>().ShowNextSceneDirection(Vector2.right);
        else if (dir.x < dir.y && dir.x < -2) GetComponent<DirectionArrows>().ShowNextSceneDirection(Vector2.left);
    }

    public void HideDirectionArrows()
    {
        DirectionArrows da = GetComponent<DirectionArrows>();
        if (!da) return;

        da.HideArrows();
    }
}
