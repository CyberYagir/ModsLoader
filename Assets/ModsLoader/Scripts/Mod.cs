using System;
using System.Collections.Generic;
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

    public Assembly assembly;
    
    
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
    
    public int GetScenesCount => scenes.Count;
    public int GetAssetsCount => bundle.LoadAllAssets<Object>().Length;

    public void LoadSceneFromAsset(int id)
    {
        if (GetScenesCount == 0) return;
        id = Mathf.Clamp(id, 0, GetScenesCount);
        SceneManager.LoadScene(scenes[id]);
    }

    public void AddLoader()
    {
        var holder = new GameObject(data.modName);
        foreach (var t in data.initializers.namedScripts)
        {
            holder.AddComponent(assembly.GetType((string)t));
        }
    }
    
    
    
}