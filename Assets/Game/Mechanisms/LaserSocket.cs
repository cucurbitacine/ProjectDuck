using Game.Abilities.Laser;
using UnityEngine;

namespace Game.Mechanisms
{
    public class LaserSocket : SocketBase, ILaserHandler
    {
        [SerializeField] private float timeout = 0.1f;

        private float _timer;
        
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