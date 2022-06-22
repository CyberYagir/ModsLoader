using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// This class collects a list of mods, unzips the mods into mod folders, then loads the mods in the order chosen by the player. After that, it creates mod objects and remembers them.
/// </summary>
public class ModsLoader
{
    /// <summary>
    /// List of mods Paths
    /// </summary>
    public List<string> modArchives { get; private set; } 
    
    /// <summary>
    /// User mods load queue
    /// </summary>
    public ModsLoaderChain loadChain  { get; private set; } = new ModsLoaderChain();

    /// <summary>
    /// List of loaded mods
    /// </summary>
    public List<Mod> mods { get; private set; } = new List<Mod>();

    /// <summary>
    /// Initialization unpacks mods and converts their files to Mod
    /// </summary>
    /// <param name="modsFolder"></param>
    /// <param name="modsChainFolder"></param>
    /// <param name="loader">Mod Assembly Loader</param>
    public void Init(string modsFolder, string modsChainFolder, ModAssemblyLoader loader)
    {
        var unpackPath = modsFolder + "/Unpacked/";
        modArchives = GetModsList(modsFolder);
        loadChain = ModsLoaderChain.LoadFile(modsChainFolder);
        loadChain = InitChain(modArchives, loadChain, modsChainFolder);
        
        var unpackedFolder = UnpackMods(unpackPath, modArchives);

        loader.Init(unpackPath);

        mods = GetAssetBundles(unpackedFolder, loader, loadChain);

        UnloadMods(unpackPath);
    }

    public List<string> UnpackMods(string unpackPath, List<string> modFiles)
    {
        Directory.CreateDirectory(unpackPath);

        var unpackedDirectories = new List<string>();
        
        for (int i = 0; i < modFiles.Count; i++)
        {
            var tempModPath = unpackPath + "/" + Path.GetFileNameWithoutExtension(modFiles[i]);
            Directory.CreateDirectory(tempModPath);
            ArchiveUtility.ExtractZipContent(modFiles[i], null, tempModPath);
            unpackedDirectories.Add(tempModPath);
        }

        return unpackedDirectories;
    }

    public void UnloadMods(string unpackPath)
    {
        Directory.Delete(unpackPath, true);
    }

    public void UnloadBundles()
    {
        foreach (var mod in mods)
        {
            mod.Unload();
        }
    }

    public List<string> GetModsList(string modsFolder)
    {
        var mods = Directory.GetFiles(modsFolder, "*.modFile", SearchOption.AllDirectories).ToList();
        return mods;
    }

    public List<Mod> GetAssetBundles(List<string> paths, ModAssemblyLoader assemblyLoader, ModsLoaderChain chain)
    {
        List<Mod> mods = new List<Mod>();

        var chainPaths = new List<string>();
        for (int i = 0; i < chain.mods.Count; i++)
        {
            for (int j = 0; j < paths.Count; j++)
            {
                if (Path.GetFileNameWithoutExtension(chain.mods[i]) == Path.GetFileNameWithoutExtension(paths[j]))
                {
                    chainPaths.Add(paths[j]);
                    break;
                }
            }
        }

        for (int i = 0; i < chainPaths.Count; i++)
        {
            var modName = Path.GetFileNameWithoutExtension(chainPaths[i]);
            var modFile = chainPaths[i] + "/" + modName;


            var modPath = modFile + ".mod";
            var modExPath = modFile + ".modEx";


            AssetBundle asset = null, scenes = null;

            if (File.Exists(modPath))
            {
                asset = AssetBundle.LoadFromFile(modPath);
            }

            if (File.Exists(modExPath))
            {
                scenes = AssetBundle.LoadFromFile(modExPath);
            }

            Assembly currentDll = null;
            if (assemblyLoader.loadedAssembles.ContainsKey(modName))
            {
                currentDll = assemblyLoader.loadedAssembles[modName];
            }

            var mod = new Mod(asset, scenes, currentDll);

            mods.Add(mod);
        }

        return mods;
    }

    public ModsLoaderChain InitChain(List<string> mods, ModsLoaderChain chain, string modsChainFolder)
    {
        if (chain == null)
        {
            chain = new ModsLoaderChain
            {
                mods = mods
            };
        }
        else
        {
            var oldMods = chain.mods;
            for (int i = 0; i < oldMods.Count; i++)
            {
                if (!File.Exists(oldMods[i]))
                {
                    chain.mods.Remove(oldMods[i]);
                }
            }
            for (int i = 0; i < mods.Count; i++)
            {
                if (!chain.mods.Contains(mods[i]))
                {
                    chain.mods.Add(mods[i]);
                }
            }
        }

        chain.SaveFile(modsChainFolder);

        return chain;
    }
    
    
}