using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScriptData
{
    public List<TextAsset> scripts = new List<TextAsset>();
    public List<string> namedScripts = new List<string>();

    public void UpdateAll(List<string> classNames)
    {
        namedScripts = new List<string>();
        foreach (var script in classNames)
        {
            if (script != null && !namedScripts.Contains(script))
                namedScripts.Add(script);
        }
    }
}