using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


/// <summary>
/// The mod class is needed so that the developer can understand what data is stored where, can use the data of other mods, and much more.
/// The mod class also organizes and structures the data, and also makes it easy to work with the data.
/// </summary>
public class Mod
{
    /// <summary>
    /// Mod Scriptable Object reference 
    /// </summary>
    public ModDataObject data { get; private set; }

    /// <summary>
    /// Mod AssetBundle (not streaming, assets only)
    /// </summary>
    public AssetBundle bundle { get; private set; }

    /// <summary>
    /// Mod AssetBundle streaming, have scenes
    /// </summary>
    public AssetBundle scenesBundle { get; private set; }

    /// <summary>
    /// Mod Classes
    /// </summary>
    public Assembly assembly { get; private set; }

    /// <summary>
    /// List of scenes inside list
    /// </summary>
    private List<string> scenes = new List<string>();

    /// <summary>
    /// Scenes Count
    /// </summary>
    public int GetScenesCount => scenes.Count;

    /// <summary>
    /// Assets Count
    /// </summary>
    public int GetAssetsCount => bundle.LoadAllAssets<Object>().Length;

    /// <summary>
    /// Mod Class Constructor
    /// </summary>
    /// <param name="bundle">Assets AssetBundle</param>
    /// <param name="scenesBundle">Streaming AssetBundle (Scenes)</param>
    /// <param name="assembly">Mod Assembly</param>
    public Mod(AssetBundle bundle, AssetBundle scenesBundle, Assembly assembly)
    {
        this.bundle = bundle;
        this.scenesBundle = scenesBundle;
        this.assembly = assembly;
        data = bundle.LoadAllAssets<ModDataObject>()[0];

        if (scenesBundle != null) //Load Scenes
        {
            scenes = scenesBundle.GetAllScenePaths().ToList();
        }

        if (assembly != null) //Load Assembly
        {
            AddLoader();
        }
    }

    /// <summary>
    /// Name of mod Asset
    /// </summary>
    /// <param name="assetName">Asset name without $...</param>
    /// <returns></returns>
    public string GetAssetName(string assetName)
    {
        return assetName + "$" + data.modName;
    }

    /// <summary>
    /// Load Scene From Asset
    /// </summary>
    /// <param name="id">Scene id in mod</param>
    public AsyncOperation LoadSceneFromAsset(int id, bool isAsync = false)
    {
        if (GetScenesCount == 0) return null;
        id = Mathf.Clamp(id, 0, GetScenesCount);
        if (isAsync)
        {
            return SceneManager.LoadSceneAsync(scenes[id]);
        }
        else
        {
            SceneManager.LoadScene(scenes[id]);
        }

        return null;
    }

    /// <summary>
    /// Load Scene From Asset
    /// </summary>
    /// <param name="assetNameWithoutModName">Scene id in mod</param>
    /// <param name="isAsync"></param>
    /// <returns></returns>
    public AsyncOperation LoadSceneFromAsset(string assetNameWithoutModName, bool isAsync = false)
    {
        var find = scenes.FindIndex(x => Path.GetFileNameWithoutExtension(x) == GetAssetName(assetNameWithoutModName) || x == assetNameWithoutModName);
        if (find != -1)
        {
            if (isAsync)
            {
                return SceneManager.LoadSceneAsync(scenes[find]);
            }
            else
            {
                SceneManager.LoadScene(scenes[find]);
            }
        }
        else if (assetNameWithoutModName != "")
        {
            Debug.LogError($"ModLoader: Scene \"{assetNameWithoutModName}\" missing");
        }

        return null;
    }

    /// <summary>
    /// Return object by name
    /// </summary>
    /// <param name="assetNameWithoutModName">Asset Name Without Mod Name (without $...)</param>
    /// <returns></returns>
    public Object GetAsset(string assetNameWithoutModName)
    {
        return bundle.LoadAsset(GetAssetName(assetNameWithoutModName));
    }

    /// <summary>
    /// Create start mod load in DontDestroyOnLoad Scene
    /// </summary>
    private void AddLoader()
    {
        var holder = new GameObject(data.modName);
        foreach (var t in data.initializers.namedScripts)
        {
            var component = holder.AddComponent(assembly.GetType((string) t));
            var init = component as ModInit;
            if (init != null)
            {
                init.Init(this);
                Object.DontDestroyOnLoad(holder);
            }
        }
    }

    /// <summary>
    /// Unload all assets
    /// </summary>
    public void Unload()
    {
        if (bundle != null)
        {
            bundle.Unload(true);
        }

        if (scenesBundle != null)
        {
            scenesBundle.Unload(true);
        }
    }
}