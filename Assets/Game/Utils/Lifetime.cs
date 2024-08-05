using UnityEngine;

namespace Game.Utils
{
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