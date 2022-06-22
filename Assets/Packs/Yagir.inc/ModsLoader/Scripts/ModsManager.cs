using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

/// <summary>
/// The mod manager is the main link that should be on the starting scene. It is the ModsManager that triggers the loading of mods and initialization.
/// </summary>
public class ModsManager : MonoBehaviour
{
    /// <summary>
    /// Singleton of ModsManager
    /// </summary>
    public static ModsManager Instance;
    /// <summary>
    /// Mods Folder Path
    /// </summary>
    public string modsFolder { get; private set; }
    /// <summary>
    /// Mods Load Chain Folder
    /// </summary>
    public string modsChainFolder { get; private set; }
    /// <summary>
    /// Loader mods
    /// </summary>
    public ModsLoader modLoader { get; private set; }
    /// <summary>
    /// Loader assembly
    /// </summary>
    public ModAssemblyLoader modAssemblyLoader { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    public void ReloadMods()
    {
        modLoader.UnloadBundles();
        Process.Start(Application.dataPath + "/../" + Application.productName + ".exe"); 
        Application.Quit();
    }

    public void Init()
    {
        modsFolder =  Application.dataPath + $"/../Mods/";
        Directory.CreateDirectory(modsFolder);
        modsChainFolder = modsFolder + "loader.chain";

        modAssemblyLoader = new ModAssemblyLoader();
        modLoader = new ModsLoader();
        
        modLoader.Init(modsFolder, modsChainFolder, modAssemblyLoader);
    }

    private void OnApplicationQuit()
    {
        for (int i = 0; i < modLoader.mods.Count; i++)
        {
            modLoader.mods[i].bundle?.Unload(true);
            modLoader.mods[i].scenesBundle?.Unload(true);
        }
    }
}
