using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All mods that should work immediately after the start of the game, or before the first frame is displayed, must have a loader that must be inherited from this class.
/// </summary>
public abstract class ModInit : MonoBehaviour
{
    /// <summary>
    /// Mod of this Loader
    /// </summary>
    public Mod rootMod { get; private set; }

    public void Init(Mod mod)
    {
        rootMod = mod;
    }

    public virtual void Awake()
    {
    }

    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }
}
