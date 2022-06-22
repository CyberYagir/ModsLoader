using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ModsLoaderChain
{
    public List<string> mods = new List<string>();
    private string lastPath;
    public static ModsLoaderChain LoadFile(string path)
    {
        ModsLoaderChain chain = new ModsLoaderChain();
        if (File.Exists(path))
        {
            try
            {
                chain = JsonUtility.FromJson<ModsLoaderChain>(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                Debug.LogError("ModLoader: Chain error " + e.Message);
            }
        }
        return chain;
    }

    public void MoveModUp(string modName)
    {
        var index = mods.FindIndex(x => Path.GetFileNameWithoutExtension(modName) == Path.GetFileNameWithoutExtension(x));
        if (index > 0)
        {
            var item = mods[index];
            mods.RemoveAt(index);
            mods.Insert(index - 1, item);
        }
        SaveFile(lastPath);
    }
    public void MoveModDown(string modName)
    {
        var index = mods.FindIndex(x => Path.GetFileNameWithoutExtension(modName) == Path.GetFileNameWithoutExtension(x));
        if (index < mods.Count-1)
        {
            var item = mods[index];
            mods.RemoveAt(index);
            mods.Insert(index + 1, item);
        }
        SaveFile(lastPath);
    }
    
    public void SaveFile(string path)
    {
        lastPath = path;
        File.WriteAllText(path, JsonUtility.ToJson(this));
    }
}