using System;
using UnityEngine;

namespace Game.Abilities.Laser
{
    public class LaserSocket : MonoBehaviour, ILaserHandler
    {
        [SerializeField] private bool _isOn = false;
        [SerializeField] private float timeout = 0.1f;

        private float _timer;
        
        public event Action<bool> OnStateChanged; 
        
        public bool isOn
        {
            get => _isOn;
            private set
            {
                if (_isOn == value) return;
                _isOn = value;
                OnStateChanged?.Invoke(_isOn);
            }
        }

        public bool IsReflector => false;
        
        public void Impact(Vector2 point, float value)
        {
            isOn = true;
            
            _timer = timeout;
        }

        private void HandleTimeout(float deltaTime)
        {
            if (_timer > 0f)
            {
                _timer -= deltaTime;
            }
            else
            {
                isOn = false;
            }
        }
        
        private void Update()
        {
            if (isOn)
            {
                HandleTimeout(Time.deltaTime);
            }
        }
    }
}