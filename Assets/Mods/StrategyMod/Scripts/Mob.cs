using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class Mob : MonoBehaviour
    {
        [SerializeField] private float minDistance;
        [SerializeField] private float speed;
        private void Update()
        {
            if (Manager.Instance.GetPlayer() == null) return;
            var pPos = Manager.Instance.GetPlayer().position;
            if (Vector3.Distance(transform.position, pPos) > minDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, Manager.Instance.GetPlayer().position, speed * Time.deltaTime);
            }
            transform.LookAt(new Vector3(pPos.x, transform.position.y, pPos.z));
        }
    }
}
