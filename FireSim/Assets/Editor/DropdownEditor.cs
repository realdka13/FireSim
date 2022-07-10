
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SparkPoint))]
public class DropdownEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SparkPoint script = (SparkPoint)target;

        GUIContent arrayLabel = new GUIContent("FuelType");
        script.arrayIdx = EditorGUILayout.Popup(arrayLabel, script.arrayIdx, script.FuelType);
    }
}