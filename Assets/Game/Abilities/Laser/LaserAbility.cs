using System.Collections.Generic;
using System.Linq;
using CucuTools.DamageSystem;
using Game.Utils;
using Inputs;
using UnityEngine;

namespace Game.Abilities.Laser
{
    public class LaserAbility : AbilityBase, IPaused
    {
        [field: SerializeField] public bool Paused { get; private set; }
        
        [Header("Settings")]
        [Min(0)]
        [SerializeField] private float laserPower = 1f;
        [Min(0f)] [SerializeField] private float laserStartWidth = 0.01f;
        [Min(0f)] [SerializeField] private float laserEndWidth = 0.1f;
        [Min(0f)] [SerializeField] private float pushPower = 0f;
        
        [Space]
        [SerializeField] private Vector2 offset = Vector2.up * 0.5f;
        [Min(0f)] [SerializeField] private float nearPlane = 0.5f;
        [Min(0f)] [SerializeField] private float laserDistance = 10f;
        [SerializeField] private LayerMask layerMask = 1;
        [Min(0f)] [SerializeField] private float threshold = 0.001f;
        
        [Header("FX")]
        [SerializeField] private GameObject hitEffectPrefab;
        
        [Header("References")]
        [SerializeField] private LineRenderer line;
        [SerializeField] private DamageSource source;
        
        [Header("Input")]
        [SerializeField] private bool primaryFire;
        
        private PlayerInput _playerInput;
        private ContactFilter2D _filter2d = default;
        
        private readonly List<RaycastHit2D> _hits = new List<RaycastHit2D>();
        private readonly List<Transform2D> laserPoints = new List<Transform2D>();
        private readonly List<GameObject> _hitEffects = new List<GameObject>();
        
        private Vector2 worldPoint => _playerInput ? _playerInput.WorldPoint : transform.position;

        private Vector2 laserCenter => Player
            ? Player.position + (Vector2)Player.transform.TransformVector(offset)
            : transform.position + transform.TransformVector(offset);
        private Vector2 laserDirection => (worldPoint - laserCenter).normalized;
        private Vector2 laserOrigin => laserCenter + laserDirection * nearPlane;
        
        public void Pause(bool value)
        {
            Paused = value;
        }
        
        private void HandlePrimaryFire(bool value)
        {
            primaryFire = value;
        }
        
        private RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance)
        {
            _filter2d.useTriggers = false;
            _filter2d.useLayerMask = true;
            _filter2d.layerMask = layerMask;

            var count = Physics2D.Raycast(origin, direction, _filter2d, _hits, distance);

            return count > 0 ? _hits[0] : default;

            //return Physics2D.Raycast(origin, direction, distance, layerMask);
        }
        
        private void EvaluateLaserPoints(ICollection<Transform2D> points, float deltaTime)
        {
            points.Clear();

            if (Paused) return;
            
            var isValidPoint = Vector2.Distance(worldPoint, laserCenter) > nearPlane;

            if (!isValidPoint) return;
            if (!primaryFire) return;
            
            var origin = laserOrigin;
            var direction = laserDirection;
            var distance = laserDistance - nearPlane;
            
            points.Add(new Transform2D() { point = origin, normal = direction });

            const int limit = 100;
            for (var i = 0; i < limit; i++)
            {
                var hit = Raycast(origin, direction, distance);
                
                if (hit)
                {
                    if (hit.collider.TryGet<DamageReceiver>(out var receiver))
                    {
                        var damage = source.CreateDamage(receiver);
                        source.SendDamage(damage, receiver);
                    }

                    if (hit.rigidbody && pushPower > 0f)
                    {
                        hit.rigidbody.AddForceAtPosition(direction * pushPower, hit.point, ForceMode2D.Force);
                    }
                    
                    points.Add(new Transform2D() { point = hit.point, normal = hit.normal });
                    
                    direction = Vector2.Reflect(direction, hit.normal);
                    origin = hit.point + direction * threshold;
                    distance -= (hit.distance + threshold);

                    var handlers = hit.collider.GetComponents<ILaserHandler>();
                    
                    if (handlers == null || handlers.Length == 0) return;

                    foreach (var handler in handlers)
                    {
                        handler.Impact(hit.point, laserPower * deltaTime);
                    }

                    if (handlers.All(h => !h.IsReflector)) return;

                    if (distance <= 0f) return;
                }
                else
                {
                    points.Add(new Transform2D() { point = origin + direction * distance, normal = direction });
                    
                    return;
                }
            }
        }
        
        private void BuildLine(IReadOnlyList<Transform2D> points)
        {
            if (!line) return;

            if (hitEffectPrefab)
            {
                var newHitsAmount = points.Count - _hitEffects.Count;
                if (newHitsAmount > 0)
                {
                    for (var i = 0; i < newHitsAmount; i++)
                    {
                        var effect = Instantiate(hitEffectPrefab, transform);
                        effect.SetActive(false);
                        
                        _hitEffects.Add(effect);
                    }
                }
            }

            line.startWidth = laserStartWidth;
            line.endWidth = laserEndWidth;
            
            line.enabled = points.Count > 1;

            line.positionCount = points.Count;
            for (var i = 0; i < points.Count; i++)
            {
                line.SetPosition(i, points[i].point);

                if (!hitEffectPrefab) continue;
                if (!_hitEffects[i]) continue;

                _hitEffects[i].SetActive(i > 0);
                _hitEffects[i].transform.position = points[i].point;
                _hitEffects[i].transform.rotation = i == 0
                    ? Quaternion.LookRotation(Vector3.forward, points[i + 1].point - points[i].point)
                    : points[i].rotation;
            }

            for (var i = points.Count; i < _hitEffects.Count; i++)
            {
                _hitEffects[i]?.SetActive(false);
            }
        }
        
        protected override void OnSetPlayer()
        {
            _playerInput = Player.GetPlayerInput();
            
            _playerInput.PrimaryFireEvent += HandlePrimaryFire;
            
            source.SetOwner(Player.gameObject);
            
            Player.Health.OnDied += HandleDeath;
        }

        private void HandleDeath()
        {
            Pause(true);
        }

        #region MonoBehaviour

        private void OnEnable()
        {
            if (line)
            {
                line.enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (_playerInput)
            {
                _playerInput.PrimaryFireEvent -= HandlePrimaryFire;
            }

            if (Player && Player.Health)
            {
                Player.Health.OnDied -= HandleDeath;
            }
        }
        
        private void FixedUpdate()
        {
            EvaluateLaserPoints(laserPoints, Time.fixedDeltaTime);
            
            BuildLine(laserPoints);
        }

        private void OnValidate()
        {
            if (line)
            {
                line.startWidth = laserStartWidth;
                line.endWidth = laserEndWidth;
            }
        }

        private void OnDrawGizmos()
        {
            var isValidPoint = Vector2.Distance(worldPoint, laserCenter) > nearPlane;

            Gizmos.color = primaryFire ? Color.white : Color.grey;
            
            if (isValidPoint)
            {
                Gizmos.DrawWireSphere(worldPoint, 0.1f);
                
                if (primaryFire)
                {
                    Gizmos.DrawLine(laserOrigin, laserCenter + laserDirection * laserDistance);
                }
            }
            
            if (nearPlane > 0f)
            {
                Gizmos.DrawWireSphere(laserCenter, nearPlane);
            }
        }

        #endregion
    }

    public struct Transform2D
    {
        public Vector2 point;
        public Vector2 normal;

        public Quaternion rotation => Quaternion.LookRotation(Vector3.forward, normal);
    }
}