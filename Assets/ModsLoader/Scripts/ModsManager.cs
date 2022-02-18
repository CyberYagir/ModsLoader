using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class ModsManager : MonoBehaviour
{
    public static ModsManager Instance;
    public string modsFolder { get; private set; }
    public string modsChainFolder { get; private set; }
    public ModsLoader modLoader { get; private set; }
    public ModAssemblyLoader modAssemblyLoader { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
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
}