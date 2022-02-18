using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScriptData
{
    [SerializeField] private List<TextAsset> scripts = new List<TextAsset>();
    public List<string> namedScripts = new List<string>();

    public void UpdateAll()
    {
        namedScripts = new List<string>();
        foreach (var script in scripts)
        {
            if (script != null && !namedScripts.Contains(script.name))
                namedScripts.Add(script.name);
        }
    }
}