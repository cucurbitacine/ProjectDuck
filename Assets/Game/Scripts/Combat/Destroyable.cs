using UnityEngine;

namespace Game.Scripts.Combat
{
    [RequireComponent(typeof(Health))]
    public class Destroyable : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool destroyObject = false;
        
        [Header("References")]
        [SerializeField] private GameObject deathEffectPrefab;
        
        private Health _health;

        private void HandleDeath()
        {
            if (deathEffectPrefab)
            {
                Instantiate(deathEffectPrefab, transform.position, transform.rotation);
            }

            if (destroyObject)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnDied += HandleDeath;
        }

        private void OnDisable()
        {
            _health.OnDied -= HandleDeath;
        }
    }
}