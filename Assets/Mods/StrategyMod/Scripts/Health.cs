using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FPS
{
    public class Health : MonoBehaviour
    {
        
        [SerializeField] private int health = 1;
        private int maxHP;
        public UnityEvent OnDamage, OnDeath;

        private void Start()
        {
            maxHP = health;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            OnDamage.Invoke();
            if (health <= 0)
            {
                OnDeath.Invoke();
                Destroy(gameObject);
            }
        }

        public float GetPercent()
        {
            return health / (float) maxHP;
        }
    }
}
