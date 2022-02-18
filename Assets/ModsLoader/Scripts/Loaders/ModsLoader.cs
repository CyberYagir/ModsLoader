using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ModsLoader
{
    public List<string> modArchives { get; private set; }
    public ModsLoaderChain loadChain  { get; private set; } = new ModsLoaderChain();

    public List<Mod> mods { get; private set; } = new List<Mod>();

    public void Init(string modsFolder, string modsChainFolder, ModAssemblyLoader loader)
    {
        var unpackPath = modsFolder + "/Unpacked/";
        modArchives = GetModsList(modsFolder);
        loadChain = ModsLoaderChain.LoadFile(modsChainFolder);
        loadChain = InitChain(modArchives, loadChain, modsChainFolder);
        
        var unpackedFolder = UnpackMods(unpackPath, modArchives);

        loader.Init(unpackPath);

        mods = GetAssetBundles(unpackedFolder, loader);

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

    public List<string> GetModsList(string modsFolder)
    {
        var mods = Directory.GetFiles(modsFolder, "*.modFile", SearchOption.AllDirectories).ToList();
        return mods;
    }
    public List<Mod> GetAssetBundles(List<string> paths, ModAssemblyLoader assemblyLoader)
    {
        List<Mod> mods = new List<Mod>();
        for (int i = 0; i < paths.Count; i++)
        {
            var modName = Path.GetFileNameWithoutExtension(paths[i]);
            var modFile = paths[i] + "/" + modName;


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