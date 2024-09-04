using Game.Scripts.Interactions;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Abilities.Laser
{
    public class LaserSocket : ToggleBase, ILaserHandler
    {
        [Header("Laser Socket")]
        [SerializeField] private float timeout = 0.1f;
        [SerializeField] private bool onlyOnce = false;

        [Space]
        [SerializeField] private UnityEvent onHit = new UnityEvent();
        [SerializeField] private UnityEvent onRelease = new UnityEvent();
            
        private float _timer;
        
        public bool IsReflector => false;
        
        public void Impact(Vector2 point, float value)
        {
            TurnOn(true);
            
            _timer = timeout;
        }

        private void HandleTimeout(float deltaTime)
        {
            if (onlyOnce) return;
            
            if (_timer > 0f)
            {
                _timer -= deltaTime;
            }
            else
            {
                TurnOn(false);
            }
        }
        
        private void HandleToggle(bool value)
        {
            if (value)
            {
                onHit.Invoke();
            }
            else
            {
                onRelease.Invoke();
            }
        }
        
        private void Update()
        {
            if (TurnedOn)
            {
                HandleTimeout(Time.deltaTime);
            }
        }

        private void OnEnable()
        {
            OnValueChanged += HandleToggle;
        }

        private void OnDisable()
        {
            OnValueChanged -= HandleToggle;
        }
    }
}