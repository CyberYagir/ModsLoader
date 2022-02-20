using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] protected Transform point;
        [SerializeField] protected GameObject bullet;
        [SerializeField] protected float cooldown, bulletForce;
        protected float time;
        
        public void Shoot()
        {
            if (time > cooldown)
            {
                var blt = Instantiate(bullet, point.transform.position, point.transform.rotation);
                blt.GetComponent<Rigidbody>().velocity = blt.transform.forward * bulletForce;
                blt.GetComponent<Bullet>().Init(gameObject);
                time = 0;
            }
        }
    }
    
    public class PlayerGun : Gun
    {
        private void Update()
        {
            time += Time.deltaTime;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Shoot();
            }
        }
    }
}
