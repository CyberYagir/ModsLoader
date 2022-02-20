using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StategyMod
{
    public class MobGun : Gun
    {
        private void Update()
        {
            time += Time.deltaTime;
            Shoot();
        }
    }
}
