using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
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
            
            
            
            CreateAssetBundle(path, filesFolder, mod);

            CreateStreamingBundle(filesFolder, modFolder, mod);

            CopyDll(modFolder, filesFolder, mod);

            CreateArchiveFile(filesFolder, mod);
        }
    }

    public static void CreateAssetBundle(string modFolder, string filesFolder, ModDataObject mod)
    {
        var selectedObject = CollectAssets(modFolder);
        selectedObject.RemoveAll(x => x is SceneAsset);
        BuildPipeline.BuildAssetBundle(mod, selectedObject.ToArray(), filesFolder + $"{mod.modName}.mod", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows);

        Selection.activeObject = mod;
    }

    public static void CopyDll(string modFolder, string finalPath, ModDataObject mod)
    {
        var objPath = Application.dataPath + $"/../obj/";
        var allDlls = Directory.GetFiles(objPath, "*.dll", SearchOption.AllDirectories);
        
        var findedDll = allDlls.ToList().Find(x => Path.GetFileNameWithoutExtension(x) == mod.modName + "Assembly");
        
        
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

        BuildPipeline.BuildStreamedSceneAssetBundle(scenePaths.ToArray(), path + $"{mod.modName}.modEx", BuildTarget.StandaloneWindows);
    }

    public static void CreateArchiveFile(string filesFolder, ModDataObject mod)
    {
        var zipPath = filesFolder + $"/../{mod.modName}.modFile";
        ArchiveUtility.CompressDirectory(filesFolder, zipPath);
        Directory.Delete(filesFolder, true);
    }
    
    
    
}
