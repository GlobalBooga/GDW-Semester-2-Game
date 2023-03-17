#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DualPistols_Single))]
public class DualPistolsSingleEditor : Editor
{
    SerializedProperty muzzleFlash;
    SerializedProperty bullet;
    SerializedProperty bulletSpawn;
    SerializedProperty sr;


    public override void OnInspectorGUI()
    {
        DualPistols_Single dp = (DualPistols_Single)target;

        muzzleFlash = serializedObject.FindProperty("muzzleFlash");
        bullet = serializedObject.FindProperty("bullet");
        bulletSpawn = serializedObject.FindProperty("bulletSpawn");
        sr = serializedObject.FindProperty("sr");
        
        EditorGUILayout.PropertyField(muzzleFlash, new GUIContent("Muzzle Flash"));
        EditorGUILayout.PropertyField(bullet, new GUIContent("Bullet"));
        EditorGUILayout.PropertyField(bulletSpawn, new GUIContent("Bullet Spawn"));
        EditorGUILayout.PropertyField(sr, new GUIContent("Sprite Renderer"));
        
        serializedObject.ApplyModifiedProperties();
    }
}

#endif
