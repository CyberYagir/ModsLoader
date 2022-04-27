using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class BundleCreatorEditor : MonoBehaviour
{
    [MenuItem("ModLoader/Build AssetBundle from Mod")]
    public static void ExportResource()
    {
        Directory.CreateDirectory(Application.dataPath + "/Mods");
        
        string path = Application.dataPath + $"/../Mods/";
        if (Selection.activeObject is ModDataObject)
        {
            var mod = Selection.activeObject as ModDataObject;
            var modFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject)) + "/";
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(modFolder);

            var filesFolder = path + $"{mod.modName}/";
            Directory.CreateDirectory(filesFolder);


            RenameAssets(modFolder, mod);
            
            CreateAssetBundle(path, filesFolder, mod);

            CreateStreamingBundle(filesFolder, modFolder, mod);

            CopyDll(filesFolder, mod);

            CreateArchiveFile(filesFolder, mod);
        }
    }

    public static void RenameAssets(string modFolder, ModDataObject mod)
    {
        var selectedObject = CollectAssets(modFolder);
        selectedObject.RemoveAll(x => x is MonoScript || x is ModDataObject || x is AssemblyDefinitionAsset || x is DefaultAsset);
        var separator = '$';
        var endOfName = separator + mod.modName;
        for (int i = 0; i < selectedObject.Count; i++)
        {
            var assetPath = AssetDatabase.GetAssetPath(selectedObject[i]);
            if (selectedObject[i].name.Contains(separator))
            {
                var splitedName = selectedObject[i].name.Split(separator);
                var lastPart = splitedName[splitedName.Length - 1];
                if (lastPart != mod.modName)
                {
                    AssetDatabase.RenameAsset(assetPath, splitedName[0] + endOfName);
                }
            }
            else
            {
                AssetDatabase.RenameAsset(assetPath, selectedObject[i].name + endOfName);
            }
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
    
    public static void CreateAssetBundle(string modFolder, string filesFolder, ModDataObject mod)
    {
        var selectedObject = CollectAssets(modFolder);
        selectedObject.RemoveAll(x => x is SceneAsset);
#pragma warning disable 618
        BuildPipeline.BuildAssetBundle(mod, selectedObject.ToArray(), filesFolder + $"{mod.modName}.mod", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows);
#pragma warning restore 618

        Selection.activeObject = mod;
    }
    public static void CopyDll(string finalPath, ModDataObject mod)
    {
        var objPath = Application.dataPath + $"/../obj/";
        var allDlls = Directory.GetFiles(objPath, "*.dll", SearchOption.AllDirectories);
        
        var findedDll = allDlls.ToList().Find(x => Path.GetFileNameWithoutExtension(x) == mod.modName + "Assembly");
        if (string.IsNullOrEmpty(findedDll))
        {
            Debug.LogError("ModLoader: " + mod.modName + " Builds Without Dll");
            return;
        }
        
        var dllPath = Path.GetFullPath(findedDll);
        var finalDllPath = Path.GetFullPath(finalPath + $"{mod.modName}.dll");
        if (File.Exists(finalDllPath))
        {
            File.Delete(finalDllPath);
        }
        File.Copy(dllPath, finalDllPath);
    }
    public static List<Object> CollectAssets(string modFolder)
    {
        string[] selected = AssetDatabase.FindAssets("", new [] {modFolder} );
        List<Object> selectedObject = new List<Object>();

        for (int i = 0; i < selected.Length; i++)
        {
            var obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(selected[i]));
            selectedObject.Add(obj);
        }

        return selectedObject;
    }

    public static void CreateStreamingBundle(string path, string modFolder, ModDataObject mod)
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(modFolder);
        var selection = CollectAssets(modFolder);
        List<string> scenePaths = new List<string>();
        for (int i = 0; i < selection.Count; i++)
        {
            if (selection[i] is SceneAsset)
            {
                scenePaths.Add(AssetDatabase.GetAssetPath(selection[i]));
            }
        }

        if (scenePaths.Count != 0)
        {
            BuildPipeline.BuildStreamedSceneAssetBundle(scenePaths.ToArray(), path + $"{mod.modName}.modEx", BuildTarget.StandaloneWindows, BuildOptions.None);
        }
    }

    public static void CreateArchiveFile(string filesFolder, ModDataObject mod)
    {
        var zipPath = filesFolder + $"/../{mod.modName}.modFile";
        ArchiveUtility.CompressDirectory(filesFolder, zipPath);
        Directory.Delete(filesFolder, true);
    }
}
