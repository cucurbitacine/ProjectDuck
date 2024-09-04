using System.Collections.Generic;
using UnityEngine;

namespace Game.Dev
{
    public class SpawnController : MonoBehaviour
    {
        public GameObject player;

        [Space] public List<Transform> spawnPoints = new List<Transform>();

        private readonly KeyCode[] keys = new KeyCode[]
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Alpha0,
        };

        public void Spawn(Transform spawnPoint)
        {
            if (player && spawnPoint)
            {
                player.transform.SetPositionAndRotation(spawnPoint.position, player.transform.rotation);
            }
        }

        public void Spawn(int index)
        {
            if (0 <= index && index < spawnPoints.Count)
            {
                Spawn(spawnPoints[index]);
            }
        }

        private void Update()
        {
            for (var i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyDown(keys[i]))
                {
                    Spawn(i);
                }
            }
        }
    }
}