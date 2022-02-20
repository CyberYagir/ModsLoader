using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class Manager : MonoBehaviour
    {
        public static Manager Instance;
        [SerializeField] private GameObject player;

        private void Awake()
        {
            Instance = this;
            LoaderCanvas(false);
        }

        public void LoaderCanvas(bool state)
        {
            if (ModsManager.Instance != null)
            {
                var cnvs = ModsManager.Instance.GetComponentInChildren<Canvas>(true);
                if (cnvs)
                {
                    cnvs.gameObject.SetActive(state);
                }
            }
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
        }


        public Transform GetPlayer()
        {
            if (player)
            {
                return player.transform;
            }

            return null;
        }
    }
}
