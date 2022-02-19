using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanvasMod
{
    public class CanvasModLoader : ModInit
    {
        public override void Start()
        {
            base.Start();
            Instantiate(rootMod.GetAsset("Canvas"));
        }
    }
}
