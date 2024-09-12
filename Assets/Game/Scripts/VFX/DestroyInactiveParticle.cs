using UnityEngine;

namespace Game.Scripts.VFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ParticleSystem))]
    public class DestroyInactiveParticle : MonoBehaviour
    {
        private ParticleSystem _particle;

        private void Awake()
        {
            _particle = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!_particle.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}