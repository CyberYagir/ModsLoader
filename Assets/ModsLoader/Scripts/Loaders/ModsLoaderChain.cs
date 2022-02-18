using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ModsLoaderChain
{
    public List<string> mods = new List<string>();
    public static ModsLoaderChain LoadFile(string path)
    {
        ModsLoaderChain chain = new ModsLoaderChain();
        if (File.Exists(path))
        {
            try
            {
                chain = JsonUtility.FromJson<ModsLoaderChain>(path);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
        return chain;
    }

    public void SaveFile(string path)
    {
        File.WriteAllText(path, JsonUtility.ToJson(this));
    }
}