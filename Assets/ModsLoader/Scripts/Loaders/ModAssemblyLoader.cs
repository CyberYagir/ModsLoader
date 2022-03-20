using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using UnityEngine;

public class ModAssemblyLoader
{
    public Dictionary<string, Assembly> loadedAssembles { get; private set; }

    public Dictionary<string, Assembly> Init(string modsFolder)
    {
        var allDlls = FindDlls(modsFolder);
        CheckAssembly(allDlls);
        LoadAssemblies(allDlls);
        return loadedAssembles;
    }
    private void LoadAssemblies(List<string> assemblyFiles)
    {
        loadedAssembles = new Dictionary<string, Assembly>();
        foreach (string path in assemblyFiles)
        {
            if (!File.Exists(path))
                continue;

            try
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(path));
                assembly.GetTypes();
                loadedAssembles.Add(Path.GetFileNameWithoutExtension(path), assembly);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
    public List<string> FindDlls(string modsFolder)
    {
        return Directory.GetFiles(modsFolder, "*.dll", SearchOption.AllDirectories).ToList();
    }

    public void CheckAssembly(List<string> dllPaths)
    {
        for (int i = 0; i < dllPaths.Count; i++)
        {
            AssemblyDefinition assemblyDefinition = null;
            try
            {
                assemblyDefinition = Mono.Cecil.AssemblyDefinition.ReadAssembly(dllPaths[i]);
                Debug.LogWarning(Path.GetFileName(dllPaths[i]) + " loaded");
            }
            catch (Exception e)
            {
                Debug.LogWarning(Path.GetFileName(dllPaths[i]) + " have error");
                continue;
            }
        }
    }
}
