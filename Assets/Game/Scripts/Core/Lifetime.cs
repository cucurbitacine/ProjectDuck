using UnityEngine;

namespace Game.Scripts.Core
{
    [DisallowMultipleComponent]
    public class Lifetime : MonoBehaviour
    {
        [SerializeField] private float lifetime = 5f;

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
        
        private void Start()
        {
            Invoke(nameof(DestroySelf), lifetime);
        }
    }
}