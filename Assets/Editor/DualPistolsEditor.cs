using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DualPistols_Single))]
public class DualPistolsSingleEditor : Editor
{
    SerializedProperty muzzleFlash;
    SerializedProperty bullet;
    SerializedProperty bulletSpawn;


    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DualPistols_Single dp = (DualPistols_Single)target;

        muzzleFlash = serializedObject.FindProperty("muzzleFlash");
        bullet = serializedObject.FindProperty("bullet");
        bulletSpawn = serializedObject.FindProperty("bulletSpawn");
        
        
        
        EditorGUILayout.PropertyField(muzzleFlash, new GUIContent("Muzzle Flash"));
        EditorGUILayout.PropertyField(bullet, new GUIContent("Bullet"));
        EditorGUILayout.PropertyField(bulletSpawn, new GUIContent("Bullet Spawn"));
        
        serializedObject.ApplyModifiedProperties();
    }
}
