using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DualPistols))]
public class DualPistolsEditor : Editor
{
    SerializedProperty rightGun;
    SerializedProperty leftGun;
    SerializedProperty pickupIndicator;

    public override void OnInspectorGUI()
    {
        DualPistols dp = (DualPistols)target;

        rightGun = serializedObject.FindProperty("rightPistol");
        leftGun = serializedObject.FindProperty("leftPistol");
        pickupIndicator = serializedObject.FindProperty("pickupIndicator");

        EditorGUILayout.PropertyField(rightGun, new GUIContent("Right Gun"));
        EditorGUILayout.PropertyField(leftGun, new GUIContent("Left Gun"));
        EditorGUILayout.PropertyField(pickupIndicator, new GUIContent("Pickup Particles"));

        serializedObject.ApplyModifiedProperties();
    }
}
