using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.SFX.Impl
{
    public class CollisionSfx : MonoBehaviour
    {
        [Min(0f)] [SerializeField] private float timeout = 0.5f;
        [Min(0f)] [SerializeField] private float spread = 0.0f;

        [Space]
        [Min(0f)] [SerializeField] private float minVelocity = 2f;

        [Header("SFX")]
        [SerializeField] private SoundFX sfx;

        private float _lastTime;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.relativeVelocity.magnitude < minVelocity) return;
            
            var time = Time.time;
            var timeSinceLast = time - _lastTime;
            var spreadTimeout = timeout + Random.value * spread;
            
            if (timeSinceLast < spreadTimeout) return;
            
            sfx.PlaySfx();
            _lastTime = time;
        }
    }
}