using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private float sence;
        private float x;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sence, 0);
            x += -Input.GetAxis("Mouse Y") * sence;
            x = Mathf.Clamp(x, -90, 90);

            camera.transform.localRotation = Quaternion.Euler(x, 0, 0);
        }
    }
}
