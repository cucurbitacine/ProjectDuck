using System.Collections.Generic;
using Game.Scripts.SFX;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Abilities.Electricity
{
    public class ElectricityStrikeEffect : MonoBehaviour
    {
        public enum GenerateMode
        {
            Performance,
            Quality,
        }
        
        [field: SerializeField] public bool IsPlaying { get; private set; }
        
        [Space]
        [SerializeField] private Vector2 origin = Vector2.zero;
        [SerializeField] private Vector2 target = Vector2.one;

        [Space]
        [SerializeField] private GenerateMode generateMode = GenerateMode.Performance;
        [SerializeField] [Min(2)] private int positionCount = 2;
        [SerializeField] [Min(0f)] private float startWidth = 0.1f;
        [SerializeField] [Min(0f)] private float endWidth = 0.02f;
        
        [Space]
        [SerializeField] [Min(0f)] private float randomRadius = 0.5f;

        [Header("SFX")]
        [SerializeField] private SoundFX sfx;
        
        [Header("Time")]
        [SerializeField] [Min(0.001f)] private float rateUpdate = 1f;
        [SerializeField] [Min(0f)] private float delay = 0.25f;
        
        [Header("References")]
        [SerializeField] private LineRenderer line;

        private float periodUpdate => 1f / rateUpdate;
        
        private float _timerUpdate = 0f;
        private float _timerDelay = 0f;

        public void Play()
        {
            _timerDelay = delay;

            if (sfx)
            {
                if (!IsPlaying)
                {
                    sfx.Play(); 
                }
            }
            
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
            
            if (sfx)
            {
                sfx.Stop();
            }
        }

        public void SetPositions(Vector2 positionA, Vector2 positionB)
        {
            origin = positionA;
            target = positionB;
        }
        
        private void BuildLine(GenerateMode mode)
        {
            line.useWorldSpace = true;
            
            line.startWidth = startWidth;
            line.endWidth = endWidth;

            if (mode == GenerateMode.Quality)
            {
                var points = new List<Vector2> { origin };
                while (Vector2.Distance(points[points.Count - 1], target) > randomRadius * 2f && points.Count < positionCount)
                {
                    var point = points[points.Count - 1] + (target - points[points.Count - 1]).normalized * randomRadius;
                    point += Random.insideUnitCircle * randomRadius;
                    points.Add(point);
                }
            
                points.Add(target);

                line.positionCount = points.Count;
                for (var i = 0; i < line.positionCount; i++)
                {
                    line.SetPosition(i, points[i]);
                }
            }
            else if (mode == GenerateMode.Performance)
            {
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
            }
            else
            {
                line.positionCount = 2;
                line.SetPosition(0, origin);
                line.SetPosition(1, target);
            }
        }

        private void UpdateDelay(float deltaTime)
        {
            if (IsPlaying)
            {
                if (_timerDelay < 0)
                {
                    IsPlaying = false;
                    
                    if (sfx)
                    {
                        sfx.Stop();
                    }
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
                
                    BuildLine(generateMode);
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
            if (line && !Application.isPlaying)
            {
                BuildLine(generateMode);
            }
        }
    }
}