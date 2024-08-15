using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.InteractionSystem.Impl
{
    public class Object2DSpawner : MonoBehaviour
    {
        [SerializeField] private Vector2 offset = Vector2.zero;
        [SerializeField] private bool randomRotation = true;
        [SerializeField] private bool onlyOne = true;
        
        [Header("References")]
        [SerializeField] private GameObject prefabDefault;

        private GameObject _last;
        
        public GameObject Spawn(GameObject prefab)
        {
             var obj = Instantiate(prefab, GetSpawnPosition(), GetSpawnRotation());

             if (TryGetParent(out var parent))
             {
                 obj.transform.SetParent(parent, true);
             }

             return obj;
        }

        public GameObject SpawnDefault()
        {
            return Spawn(prefabDefault);
        }
        
        public void Spawn()
        {
            if (onlyOne && _last && _last.scene.isLoaded)
            {
                return;
            }
            
            _last = SpawnDefault();
        }
        
        private Vector2 GetSpawnPosition()
        {
            return transform.TransformPoint(offset);
        }

        private Quaternion GetSpawnRotation()
        {
            if (randomRotation) return Quaternion.Euler(0, 0, Random.value * 360);

            return prefabDefault ? prefabDefault.transform.rotation : Quaternion.identity;
        }
        
        private bool TryGetParent(out Transform parent)
        {
            return parent = transform.parent;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(GetSpawnPosition(), 0.05f);
        }
    }
}