using System.Collections;
using Game.Scripts.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Abilities.Telekinesis
{
    public class TelekinesisObject : MonoBehaviour, IPaused
    {
        [field: SerializeField] public bool Paused { get; private set; }
        [field: Space]
        [field: SerializeField] public bool Applying { get; private set; }
        [field: SerializeField, Min(0f)] public float Delay { get; private set; } = 0.1f;

        [Space]
        [SerializeField] private UnityEvent onStarted = new UnityEvent();
        [SerializeField] private UnityEvent onEnded = new UnityEvent();
        
        private float _timer;
        
        public void Pause(bool value)
        {
            Paused = value;
        }

        public void ApplyForce()
        {
            _timer = Delay;

            if (!Applying)
            {
                StartCoroutine(ApplyingForce());
            }
        }

        private IEnumerator ApplyingForce()
        {
            Applying = true;
            onStarted.Invoke();
            
            while (_timer > 0f)
            {
                _timer -= Time.deltaTime;
                yield return null;
            }
            
            Applying = false;
            onEnded.Invoke();
        }
    }
}