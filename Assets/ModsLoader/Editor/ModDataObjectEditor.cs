using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

[CustomEditor(typeof(ModDataObject))]
public class ModDataObjectEditor : Editor
{
    private ModDataObject mod;
    private Texture2D currentIcon;
    private void OnEnable()
    {
        mod = target as ModDataObject;
        if (currentIcon == null)
        {
            currentIcon = mod.iconData.GetTexture();
        }
    }


    public override void OnInspectorGUI()
    {
        mod.modName = EditorGUILayout.TextField("Mod Name: ", mod.modName).Replace(" ", "");
        GUILayout.Space(10);
        EditorGUI.BeginChangeCheck();
        {
            currentIcon = (Texture2D) EditorGUILayout.ObjectField("Mod Icon: ", currentIcon, typeof(Texture2D));
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (currentIcon != null)
            {
                mod.iconData.textureRaw = currentIcon.GetRawTextureData();
                mod.iconData.size = new Vector2Int(currentIcon.width, currentIcon.height);
                mod.iconData.textureFormat = (int)currentIcon.format;
            }
            else
            {
                mod.iconData.textureRaw = new byte[0];
            }
        }

        
        GUILayout.Label(mod.modName + " Description:");
        mod.modDescription = EditorGUILayout.TextArea(mod.modDescription, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 5));
        
        GUILayout.Space(10);
        GUILayout.Label(mod.modName + " Version: " + mod.modVersionData.ToString('.'));
        mod.modVersionData.SetStartDate();
        GUILayout.BeginHorizontal();
        {
            mod.modVersionData.major = EditorGUILayout.IntField(mod.modVersionData.major);
            mod.modVersionData.minor = EditorGUILayout.IntField(mod.modVersionData.minor);
            mod.modVersionData.patch = EditorGUILayout.IntField(mod.modVersionData.patch);
            mod.modVersionData.CheckFormatting();
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("initializers").FindPropertyRelative("scripts"), true);

        List<string> classNames = new List<string>();
        for (int i = 0; i < mod.initializers.scripts.Count; i++)
        {
            classNames.Add((mod.initializers.scripts[i] as MonoScript).GetClass().FullName);
        }
        
        mod.initializers.UpdateAll(classNames);
        GUILayout.Space(10);


        var pathToMod = AssetDatabase.GetAssetPath(mod);
        var directory = Path.GetDirectoryName(pathToMod);
        
        var isHaveAssembly = !CheckAssembly(directory);
        GUI.enabled = isHaveAssembly;
        var createAssemblyButton = GUILayout.Button("Create Assembly");
        if (createAssemblyButton)
        {
            CreateAssembly(directory);
        }
        
        GUI.enabled = !isHaveAssembly;
        if (GUILayout.Button("Build Assembly"))
        {
            BuildSolution();
        }
        if (GUILayout.Button("Build Mod"))
        {
            BuildMod(mod);
        }
        GUI.enabled = true;
    }

    public static void BuildMod(ModDataObject mod, bool buildSolution = true)
    {
        Selection.activeObject = mod;
        if (buildSolution)
        {
            BuildSolution();
        }
        BundleCreatorEditor.ExportResource();
    }
    
    public void CreateAssembly(string directory)
    {
        var name = mod.modName + "Assembly";

        var json = "{\"name\":\"" + name + "\",\"rootNamespace\":\""+ mod.modName + "\",\"references\":[\"ModsLoader\"],\"includePlatforms\":[],\"excludePlatforms\":[],\"allowUnsafeCode\":false,\"overrideReferences\":true,\"precompiledReferences\":[\"Mono.Cecil.dll\"],\"autoReferenced\":true,\"defineConstraints\":[],\"versionDefines\":[],\"noEngineReferences\":false}";
        
        File.WriteAllText(directory + "/" + name + ".asmdef", json);
        AssetDatabase.Refresh();
    }
    
    public bool CheckAssembly(string directory)
    {
        var assetsNear = AssetDatabase.FindAssets("", new []{ directory });
        foreach (var asset in assetsNear)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(asset);
            if (Path.GetDirectoryName(assetPath) == directory)
            {
                var findedAsset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                if (findedAsset is AssemblyDefinitionAsset)
                {                
                    return true;
                }
            }
        }

        return false;
    }


    public const string standardPath = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin\\msbuild.exe";
    public static void BuildSolution()
    {
        var solution = Application.dataPath + $"/../";
        var solitions = Directory.GetFiles(solution, "*.sln");
        if (solitions.Length != 0)
        {
            var findedSLN = Path.GetFullPath(solitions[0]);

            var path = standardPath;

            if (EditorPrefs.HasKey(MSBuildPathWindow.pathKey))
            {
                path = EditorPrefs.GetString(MSBuildPathWindow.pathKey);
            }
            var command = "/C \""+path+"\" ";
            if (!File.Exists(path))
            {
                Debug.LogError("ModLoader: MSBuild missing; Menu > Tool Configure > Set Build Path");
                return;
            }
            
            var final = command + " " + findedSLN + "";
            Command(final);
            Debug.Log("Console Command: " + final);
        }
        else
        {
            Debug.LogError("ModLoader: .SLN not exists");
        }
    }
    
    static void Command (string input)
    {
        var processInfo = new ProcessStartInfo("cmd.exe", input);
        processInfo.CreateNoWindow = false;
        processInfo.UseShellExecute = true;
 
        var process = Process.Start(processInfo);
        
        process.WaitForExit();
        process.Close();
        
        Debug.Log("Build Ended");
    }
}
