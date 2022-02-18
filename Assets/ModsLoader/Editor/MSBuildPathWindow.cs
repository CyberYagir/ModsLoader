using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MSBuildPathWindow : EditorWindow
{
    // Add menu named "My Window" to the Window menu
    [MenuItem("ModLoader/Tool Configure")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MSBuildPathWindow window = (MSBuildPathWindow) EditorWindow.GetWindow(typeof(MSBuildPathWindow));
        window.titleContent = new GUIContent("Mod Loader");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Functions: ", EditorStyles.boldLabel);
    }
}
