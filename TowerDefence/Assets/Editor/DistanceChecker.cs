using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/10/2018
//
//******************************

public class DistanceChecker : EditorWindow {
    
    public GameObject Object1;
    public GameObject Object2;
    
    private string _DistanceResultString = "";

    [MenuItem("Window/Distance Checker")]
    public static void ShowWindow() {

        GetWindow<DistanceChecker>("Distance Checker");
    }

    public void OnGUI() {

        EditorGUILayout.Space();

        // Header/title
        EditorGUILayout.BeginVertical();
        GUILayout.Label("DISTANCE CHECKER", EditorStyles.boldLabel);
        GUILayout.Label("- Created by Daniel Marton, 23/10/2018", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Object references
        EditorGUILayout.BeginVertical();
        Object1 = (GameObject)EditorGUILayout.ObjectField("Object 1", Object1, typeof(Object), true);
        Object2 = (GameObject)EditorGUILayout.ObjectField("Object 2", Object2, typeof(Object), true);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();

        // Distance result button
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Check Distances", GUILayout.Width(200))) {

            // Notify if theres a missing object reference
            if (Object1 == null || Object2 == null) {

                ShowNotification(new GUIContent("You are missing transform reference(s) for the distance checker. " +
                                                "Have you filled both transforms references with valid gameobjects?"));
            }

            // Determine distance and print the result
            if (Object1 != null && Object2 != null) {
                
                float dist = Vector3.Distance(Object1.transform.position, Object2.transform.position);
                _DistanceResultString = dist.ToString();
            }
            else { _DistanceResultString = ""; }
        }

        // Print the result
        EditorGUILayout.TextArea(" Result: " + _DistanceResultString, GUIStyle.none);
        EditorGUILayout.EndVertical();
    }

}
