using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FPS
{
    public class MobSpawn : MonoBehaviour
    {
        [SerializeField] private List<GameObject> points;
        [SerializeField] private GameObject prefab;
        [SerializeField] private float cooldown;

        private float time;
        private void Update()
        {
            time += Time.deltaTime;
            if (time > cooldown)
            {
                var id = Random.Range(0, points.Count);
                Instantiate(prefab, points[id].transform.position, Quaternion.identity);
                time = 0;
            }
        }
    }
}
