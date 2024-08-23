using Game.SFX;
using UnityEngine;

namespace Game.Combat
{
    [RequireComponent(typeof(Health))]
    public class Destroyable : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool destroyObject = false;
        [SerializeField] private SoundProfile destroySound;
        
        [Header("References")]
        [SerializeField] private GameObject deathEffectPrefab;
        
        private Health _health;

        private void HandleDeath()
        {
            if (deathEffectPrefab)
            {
                var deathEffect = Instantiate(deathEffectPrefab, transform.position, transform.rotation);

                if (destroySound)
                {
                    var sfx = deathEffect.GetComponentInChildren<SoundFX>();
                    if (sfx)
                    {
                        sfx.SetSoundProfile(destroySound);
                        sfx.PlaySfx();
                    }
                }
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