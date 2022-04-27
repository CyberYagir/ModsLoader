using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using File = UnityEngine.Windows.File;
using Object = UnityEngine.Object;

public class MSBuildPathWindow : EditorWindow
{
    
    public enum WindowPage
    {
        Main, SetPath, Credits, BuildMods
    }

    public WindowPage state;
    // Add menu named "My Window" to the Window menu
    [MenuItem("ModLoader/Tool Configure")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MSBuildPathWindow window = (MSBuildPathWindow) EditorWindow.GetWindow(typeof(MSBuildPathWindow));
        window.titleContent = new GUIContent("Mod Loader");
        window.Show();
        
        window.maxSize = new Vector2(350, 150);
        window.minSize = new Vector2(350, 150);
    }

    void OnGUI()
    {
        GUILayout.Label("Page: " + state.ToString(), EditorStyles.boldLabel);
        GUILayout.Space(20);
        switch (state)
        {
            case WindowPage.Main:
                MainPage();
                break;
            case WindowPage.SetPath:
                SetMSBuildPathPage();
                break;
            case WindowPage.Credits:
                if (GUILayout.Button("Github: https://github.com/CyberYagir", GUI.skin.label))
                {
                    Application.OpenURL("https://github.com/CyberYagir");
                }
                break;
            case WindowPage.BuildMods:
                BuildMods();
                break;
            default:
                state = WindowPage.Main;
                break;
        }

        if (state != WindowPage.Main)
        {
            if (GUILayout.Button("Back"))
            {
                state = WindowPage.Main;
            }
        }
    }

    public const string pathKey = "ModLoaderBuildPath";
    private string userPath = "";

    private Vector2 scroll;
    public void BuildMods()
    {
        GUI.enabled = false;
        scroll = GUILayout.BeginScrollView(scroll);
        {
            for (int i = 0; i < modsFiles.Count; i++)
            {
                modsFiles[i] = EditorGUILayout.ObjectField((modsFiles[i] as ModDataObject)?.modName, modsFiles[i], typeof(ModDataObject), allowSceneObjects: false);
            }

            GUI.enabled = true;
        }
        GUILayout.EndScrollView();

        if (GUILayout.Button("Build All"))
        {
            for (int i = 0; i < modsFiles.Count; i++)
            {
                ModDataObjectEditor.BuildMod(modsFiles[i] as ModDataObject);
            }
        }
    }
    
    public void SetMSBuildPathPage()
    {
        var path = ModDataObjectEditor.standardPath;
        if (EditorPrefs.HasKey(pathKey))
            path = EditorPrefs.GetString(pathKey);

        GUILayout.Label("MSBuild path: ");
        if (userPath == "")
        {
            userPath = path;
        }

        userPath = EditorGUILayout.TextField(userPath);

        GUI.enabled = path != userPath;

        if (GUILayout.Button("Apply"))
        {
            GUI.FocusControl(null);

            if (!File.Exists(userPath))
            {
                EditorPrefs.SetString(pathKey, ModDataObjectEditor.standardPath);
                Debug.LogError("ModLoader: File not exists");
            }
            else
            {
                if (Path.GetFileNameWithoutExtension(userPath) == "MSBuild")
                {
                    EditorPrefs.SetString(pathKey, userPath);
                }
                else
                {
                    Debug.LogError("ModLoader: Name of file not MSBuild");
                }
            }
        }

        GUI.enabled = true;
        if (GUILayout.Button("Reset"))
        {
            ResetPath();
        }
    }

    public void ResetPath()
    {
        GUI.FocusControl(null);
        EditorPrefs.SetString(pathKey, ModDataObjectEditor.standardPath);
        userPath = "";
    }

    private List<Object> modsFiles = new List<Object>();
    public void MainPage()
    {
        if (GUILayout.Button("Set Build Path"))
        {
            userPath = EditorPrefs.GetString(pathKey);
            state = WindowPage.SetPath;
        }
        if (GUILayout.Button("Credits"))
        {
           state = WindowPage.Credits;
        }

        if (GUILayout.Button("Build Mods"))
        {
            modsFiles = new List<Object>();
            string[] guids = AssetDatabase.FindAssets("t:ModDataObject", null);
            foreach (string guid in guids)
            {
                modsFiles.Add(AssetDatabase.LoadAssetAtPath<ModDataObject>(AssetDatabase.GUIDToAssetPath(guid)));
            }

            state = WindowPage.BuildMods;
        }
    }
}
