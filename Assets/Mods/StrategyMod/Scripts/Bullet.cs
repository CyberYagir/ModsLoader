using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private GameObject particles;
        private GameObject sender;
        public void Init(GameObject sender)
        {
            this.sender = sender;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger == false && other.gameObject != sender)
            {
                if (other.transform.GetComponent<Health>())
                {
                    other.transform.GetComponent<Health>().TakeDamage(1);
                }

                particles.transform.parent = null;
                particles.transform.localScale = Vector3.one / 2;
                particles.SetActive(true);
                Destroy(particles, 2f);
                Destroy(gameObject);
            }
        }
    }
}
