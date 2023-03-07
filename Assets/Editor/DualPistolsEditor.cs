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
    SerializedProperty isAutoUse;
    SerializedProperty cameraShakeIntensity;
    SerializedProperty cameraShakeTime;

    public override void OnInspectorGUI()
    {
        DualPistols dp = (DualPistols)target;

        rightGun = serializedObject.FindProperty("rightPistol");
        leftGun = serializedObject.FindProperty("leftPistol");
        pickupIndicator = serializedObject.FindProperty("pickupIndicator");
        isAutoUse = serializedObject.FindProperty("isAutoUse");
        cameraShakeIntensity = serializedObject.FindProperty("cameraShakeIntensity");
        cameraShakeTime = serializedObject.FindProperty("cameraShakeTime");

        EditorGUILayout.PropertyField(rightGun, new GUIContent("Right Gun"));
        EditorGUILayout.PropertyField(leftGun, new GUIContent("Left Gun"));
        EditorGUILayout.PropertyField(pickupIndicator, new GUIContent("Pickup Particles"));
        EditorGUILayout.PropertyField(isAutoUse, new GUIContent("Is Auto Use"));
        EditorGUILayout.PropertyField(cameraShakeIntensity, new GUIContent("Camera Shake Intensity"));
        EditorGUILayout.PropertyField(cameraShakeTime, new GUIContent("Camera Shake Time"));

        serializedObject.ApplyModifiedProperties();
    }
}
