using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Abilities.Electricity
{
    public class ElectricityStrikeEffect : MonoBehaviour
    {
        [field: SerializeField] public bool IsPlaying { get; private set; }
        
        [Space]
        [SerializeField] private Vector2 origin = Vector2.zero;
        [SerializeField] private Vector2 target = Vector2.one;

        [Space]
        [Min(2)] [SerializeField] private int positionCount = 2;
        [Min(0f)] [SerializeField] private float startWidth = 0.1f;
        [Min(0f)] [SerializeField] private float endWidth = 0.02f;
        
        [Space]
        [Min(0f)] [SerializeField] private float randomRadius = 0.5f;

        [Header("Time")]
        [Min(0.001f)] [SerializeField] private float rateUpdate = 1f;
        [Min(0f)] [SerializeField] private float delay = 0.25f;
        
        [Header("References")]
        [SerializeField] private LineRenderer line;

        private float periodUpdate => 1f / rateUpdate;
        
        private float _timerUpdate = 0f;
        private float _timerDelay = 0f;

        public void Play()
        {
            _timerDelay = delay;

            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
        }

        public void SetPositions(Vector2 positionA, Vector2 positionB)
        {
            origin = positionA;
            target = positionB;
        }
        
        private void BuildLine()
        {
            line.useWorldSpace = true;
            line.positionCount = positionCount;

            for (var i = 0; i < positionCount; i++)
            {
                var t = (float)i / (positionCount - 1);
                
                var point = Vector2.Lerp(origin, target ,t);

                if (0 < i && i < positionCount - 1)
                {
                    point += Random.insideUnitCircle * randomRadius;
                }
                
                line.SetPosition(i, point);
            }
            
            line.startWidth = startWidth;
            line.endWidth = endWidth;
        }

        private void UpdateDelay(float deltaTime)
        {
            if (IsPlaying)
            {
                if (_timerDelay < 0)
                {
                    IsPlaying = false;
                }
                else
                {
                    _timerDelay -= deltaTime;
                }
            }
        }
        
        private void UpdateLine(float deltaTime)
        {
            line.enabled = IsPlaying;
            
            if (line.enabled)
            {
                if (_timerUpdate < 0)
                {
                    _timerUpdate = periodUpdate;
                
                    BuildLine();
                }
                else
                {
                    _timerUpdate -= deltaTime;
                }
            }
        }
        
        private void Update()
        {
            UpdateDelay(Time.deltaTime);
            
            UpdateLine(Time.deltaTime);
        }

        private void OnValidate()
        {
            if (line)
            {
                BuildLine();
            }
        }
    }
}