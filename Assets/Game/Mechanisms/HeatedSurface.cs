using System;
using Game.Abilities.Laser;
using UnityEngine;

namespace Game.Mechanisms
{
    [DisallowMultipleComponent]
    public class HeatedSurface : MonoBehaviour, ILaserHandler
    {
        [SerializeField] private float heat = 0f;
        
        [Space]
        [SerializeField] private float coolingPower = 1;
        [SerializeField] private float coolingTimeout = 1f;
        
        private float _timeout = 0f;

        public bool IsReflector => false;
        
        public event Action<float> OnHeatChanged; 
        
        public float Heat
        {
            get => heat;
            private set
            {
                if (Mathf.Approximately(heat, value)) return;
                
                heat = value;
                
                OnHeatChanged?.Invoke(heat);
            }
        }

        public void Impact(Vector2 point, float value)
        {
            Heat += value;

            _timeout = coolingTimeout;
        }

        private void Update()
        {
            if (_timeout > 0)
            {
                _timeout -= Time.deltaTime;
            }
            else
            {
                if (Heat > 0f)
                {
                    Heat = Mathf.Max(0f, Heat - coolingPower * Time.deltaTime);
                }
            }
        }
    }
}