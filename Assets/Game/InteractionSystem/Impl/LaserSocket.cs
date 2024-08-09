using Game.Abilities.Laser;
using UnityEngine;

namespace Game.InteractionSystem.Impl
{
    public class LaserSocket : ToggleBase, ILaserHandler
    {
        [SerializeField] private float timeout = 0.1f;

        private float _timer;
        
        public bool IsReflector => false;
        
        public void Impact(Vector2 point, float value)
        {
            TurnOn(true);
            
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
                TurnOn(false);
            }
        }
        
        private void Update()
        {
            if (TurnedOn)
            {
                HandleTimeout(Time.deltaTime);
            }
        }
    }
}