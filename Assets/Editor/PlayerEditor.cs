#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public enum ShowData
    {
        all,
        general,
        movement,
        jumping,
        crouching,
        slope_movement,
        masks,
        objects
    }

    private ShowData showData;
    private Player player;


    [MenuItem("GameObject/Characters/Player")]
    public static void MakeNewPlayer()
    {
        GameObject player = new();
        player.name = "New Player";
        player.transform.position = Vector3.zero;

        // ADDING A SPRITE
        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("character_default");

        // ADDING A SCRIPT
        player.AddComponent<Player>();
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    /*player = (Player)target;


    showData = (ShowData)EditorGUILayout.EnumPopup("Filter", showData);

    EditorGUILayout.HelpBox("Filter what properties to show", MessageType.Info);

    EditorGUILayout.Space();


    if (GUI.changed)
    {
        switch (showData)
        {
            case ShowData.all:
                ShowGeneral();
                ShowMovement();
                ShowSlopeMovement();
                ShowJumping();
                ShowCrouching();
                ShowMasks();
                ShowObjects();
                break;
            case ShowData.general:
                ShowGeneral();
                break;
            case ShowData.movement:
                ShowMovement();
                break;
            case ShowData.jumping:
                ShowJumping();
                break;
            case ShowData.crouching:
                ShowCrouching();
                break;
            case ShowData.slope_movement:
                ShowSlopeMovement();
                break;
            case ShowData.masks:
                ShowMasks();
                break;
            case ShowData.objects:
                ShowObjects();
                break;
            default:
                break;
        }
    }

    serializedObject.ApplyModifiedProperties();*/
    }

    private void ShowGeneral()
    {
    }

    private void ShowMovement()
    {

    }

    private void ShowJumping()
    {

    }

    private void ShowCrouching()
    {

    }

    private void ShowSlopeMovement()
    {

    }

    private void ShowMasks()
    {

    }

    private void ShowObjects()
    {

    }

}

#endif