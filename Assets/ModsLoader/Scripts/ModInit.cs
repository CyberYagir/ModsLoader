using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModInit : MonoBehaviour
{
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
