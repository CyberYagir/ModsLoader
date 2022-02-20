using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StategyMod
{
    public class HealthUI : MonoBehaviour
    {
        private Health health;
        [SerializeField] private Image image;
        private void Start()
        {
            health = Manager.Instance.GetPlayer().GetComponent<Health>();
            health.OnDamage.AddListener(UpdateUI);
        }

        public void UpdateUI()
        {
            image.fillAmount = health.GetPercent();
        }
    }
}
