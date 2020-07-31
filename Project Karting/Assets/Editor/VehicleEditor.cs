using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/*
[CustomEditor(typeof(Vehicle))]
public class VehicleEditor : Editor
{
    private Material material;
    void OnEnable()
    {
        // Find the "Hidden/Internal-Colored" shader, and cache it for use.
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
        
    }

    public override void OnInspectorGUI()
    {
        GUILayout.
        var vehicle = (Vehicle) target;
        GUILayout.BeginHorizontal(EditorStyles.label);
        GUILayout.Label("Number of Gears");
        GUILayout.Space(5);
        vehicle.centerOfMass = EditorGUILayout.Transform(vehicle.centerOfMass);
    }
}
*/