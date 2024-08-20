using System.Collections.Generic;
using Game.Utils;
using Inputs;
using UnityEngine;

namespace Game.Abilities.Telekinesis
{
    public class TelekinesisAbility : AbilityBase
    {
        [Header("Settings")]
        [SerializeField] private float outerRadiusTelekinesis = 2f;
        [SerializeField] private float innerRadiusTelekinesis = 0.2f;
        [SerializeField] private LayerMask layerMask = 1;
        
        [Space]
        [Min(0f)] [SerializeField] private float minForcePower = 10f;
        [Min(0f)] [SerializeField] private float maxForcePower = 100f;
        [SerializeField] private AnimationCurve forcePowerCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("FX")]
        [Min(0f)] [SerializeField] private float particleSpeed = 1f;
        [SerializeField] private float particleSpeedScale = 1f;
        [SerializeField] private GameObject effectPrefab;
        
        [Header("Input")]
        [SerializeField] private bool primaryFire;
        
        private ParticleSystem _effect;
        private ContactFilter2D _filter2D = new ContactFilter2D();
        private PlayerInput _playerInput;
        
        private readonly List<Collider2D> _overlaps = new List<Collider2D>();
        private readonly HashSet<Rigidbody2D> _usedRigidbody2d = new HashSet<Rigidbody2D>();
        
        private Vector2 worldPoint => _playerInput ? _playerInput.WorldPoint : transform.position;
        
        private void HandlePrimaryFire(bool value)
        {
            primaryFire = value;

            Cursor.visible = !primaryFire;
            
            if (effectPrefab)
            {
                if (_effect)
                {
                    _effect.Stop();
                }
                
                if (value)
                {
                    _effect = Instantiate(effectPrefab).GetComponent<ParticleSystem>();
                
                    _effect.Play();
                }
            }
        }
        
        private float EvaluateForcePower(float t)
        {
            if (outerRadiusTelekinesis > 0 && t < innerRadiusTelekinesis / outerRadiusTelekinesis)
            {
                return 0f;
            }
            
            return forcePowerCurve.Evaluate(t) * (maxForcePower - minForcePower) + minForcePower;
        }
        
        private void ApplyForce(Rigidbody2D rigidbody2d)
        {
            if (rigidbody2d.TryGetComponent<TelekinesisObject>(out var telekinesisObject))
            {
                if (telekinesisObject.Paused) return;

                telekinesisObject.ApplyForce();
            }
            
            var distance = Vector2.Distance(worldPoint, rigidbody2d.position);
            
            var t = outerRadiusTelekinesis > 0 ? distance / outerRadiusTelekinesis : 0f;
            
            var forcePower = EvaluateForcePower(t);
            var forceDirection = (worldPoint - rigidbody2d.position).normalized;

            rigidbody2d.AddForce(forceDirection * forcePower);
        }
        
        private void ApplyForces()
        {
            _usedRigidbody2d.Clear();
            
            _filter2D.useLayerMask = true;
            _filter2D.layerMask = layerMask;
            _filter2D.useTriggers = false;
            
            var count = Physics2D.OverlapCircle(worldPoint, outerRadiusTelekinesis, _filter2D, _overlaps);
            
            for (var i = 0; i < count; i++)
            {
                var collider2d = _overlaps[i];

                if (collider2d.TryGet<Rigidbody2D>(out var rigidbody2d) && _usedRigidbody2d.Add(rigidbody2d))
                {
                    ApplyForce(rigidbody2d);
                }
            }
        }
        
        protected override void OnSetPlayer()
        {
            _playerInput = Player.GetPlayerInput();
            
            _playerInput.PrimaryFireEvent += HandlePrimaryFire;
        }
        
        private void OnDestroy()
        {
            if (_playerInput)
            {
                _playerInput.PrimaryFireEvent -= HandlePrimaryFire;
            }

            if (_effect)
            {
                _effect.Stop();
            }
            
            Cursor.visible = true;
        }

        private void Update()
        {
            if (_effect)
            {
                if (primaryFire)
                {
                    _effect.gameObject.transform.SetPositionAndRotation(worldPoint, Quaternion.identity);    
                }

                var speed = particleSpeedScale * particleSpeed;
                
                var shape = _effect.shape;
                shape.radius = outerRadiusTelekinesis;

                var main = _effect.main;
                main.startSpeed = -speed;
                main.startLifetime = (Mathf.Approximately(particleSpeed, 0f)
                    ? 1f
                    : (outerRadiusTelekinesis - innerRadiusTelekinesis) / particleSpeed);
            }
        }

        private void FixedUpdate()
        {
            if (primaryFire)
            {
                ApplyForces();
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = primaryFire ? Color.white : Color.grey;
            Gizmos.DrawWireSphere(worldPoint, outerRadiusTelekinesis);
            
            if (innerRadiusTelekinesis > 0)
            {
                Gizmos.DrawWireSphere(worldPoint, innerRadiusTelekinesis);
            }
        }
    }
}