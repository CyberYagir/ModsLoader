using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "New Mod", menuName = "ModLoader/Mod", order = 1)]
public class ModDataObject : ScriptableObject
{
    public string modName = null;
    [TextArea]
    public string modDescription = "";
    public VersionData modVersionData;
    public ScriptData initializers = new ScriptData();
    public IconData iconData;
    public List<GameObject> prefabs = new List<GameObject>();
}
