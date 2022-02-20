using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class Mod
{
    public ModDataObject data { get; private set; }
    public AssetBundle bundle { get; private set; }
    public AssetBundle scenesBundle { get; private set; }

    public Assembly assembly { get; private set; }
    
    
    private List<string> scenes = new List<string>();

    public Mod(AssetBundle bundle, AssetBundle scenesBundle, Assembly assembly)
    {
        this.bundle = bundle;
        this.scenesBundle = scenesBundle;
        this.assembly = assembly;
        data = bundle.LoadAllAssets<ModDataObject>()[0];
        
        if (scenesBundle != null)
        {
            scenes = scenesBundle.GetAllScenePaths().ToList();
        }

        if (assembly != null)
        {
            AddLoader();
        }
    }

    public string GetAssetName(string assetName)
    {
        return assetName + "$" + data.modName;
    }
    
    public int GetScenesCount => scenes.Count;
    public int GetAssetsCount => bundle.LoadAllAssets<Object>().Length;

    public void LoadSceneFromAsset(int id)
    {
        if (GetScenesCount == 0) return;
        id = Mathf.Clamp(id, 0, GetScenesCount);
        SceneManager.LoadScene(scenes[id]);
    }

    public void LoadSceneFromAsset(string assetNameWithoutModName)
    {
        var find = scenes.FindIndex(x => Path.GetFileNameWithoutExtension(x) == GetAssetName(assetNameWithoutModName) || x == assetNameWithoutModName);
        if (find != -1)
        {
            SceneManager.LoadScene(scenes[find]);
        }
        else if (assetNameWithoutModName != "")
        {
            Debug.LogError($"ModLoader: Scene \"{assetNameWithoutModName}\" missing");
        }
    }

    public Object GetAsset(string assetNameWithoutModName)
    {
        return bundle.LoadAsset(GetAssetName(assetNameWithoutModName));
    }
    
    private void AddLoader()
    {
        var holder = new GameObject(data.modName);
        foreach (var t in data.initializers.namedScripts)
        {
            var component = holder.AddComponent(assembly.GetType((string)t));
            var init = component as ModInit;
            if (init != null)
            {
                init.Init(this);
                Object.DontDestroyOnLoad(holder);
            }
        }
    }
    
    
    
}