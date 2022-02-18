using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModDrawerUI : MonoBehaviour
{
    [SerializeField] private Transform item, holder;

    private void Start()
    {
        var mods = ModsManager.Instance.modLoader.mods;
        for (int i = 0; i < mods.Count; i++)
        {
            var modItem = Instantiate(item, holder).GetComponent<ModItemUI>();
            modItem.Init(mods[i]);
            modItem.gameObject.SetActive(true);
        }
    }
}
