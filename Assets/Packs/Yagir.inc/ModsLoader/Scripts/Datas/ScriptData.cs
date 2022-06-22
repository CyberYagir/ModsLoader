using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScriptData
{
    public List<TextAsset> scripts;
    public List<string> namedScripts;

    public ScriptData()
    {
        scripts = new List<TextAsset>();
        namedScripts = new List<string>();
    }
    
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