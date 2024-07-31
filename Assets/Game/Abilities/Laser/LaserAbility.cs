using System.Collections.Generic;
using System.Linq;
using Inputs;
using UnityEngine;

namespace Game.Abilities.Laser
{
    public class LaserAbility : AbilityBase
    {
        [Header("Settings")]
        [Min(0)]
        [SerializeField] private float laserPower = 1f;
        [Min(0)]
        [SerializeField] private float laserWidth = 0.1f;
        
        [Space]
        [SerializeField] private Vector2 offset = Vector2.up * 0.5f;
        [Min(0)]
        [SerializeField] private float nearPlane = 0.5f;
        [Min(0)]
        [SerializeField] private float laserDistance = 10f;
        [SerializeField] private LayerMask layerMask = 1;
        [Min(0)]
        [SerializeField] private float threshold = 0.001f;
        
        [Header("References")]
        [SerializeField] private LineRenderer line;
        
        [Header("Input")]
        [SerializeField] private bool primaryFire;
        [SerializeField] private Vector2 screenPoint;
        
        private PlayerInput _playerInput;
        
        private readonly List<Vector3> laserPoints = new List<Vector3>();
        
        private static Camera CameraMain => Camera.main;
        
        private Vector2 worldPoint => _playerInput ? CameraMain.ScreenToWorldPoint(screenPoint) : transform.position;

        private Vector2 laserCenter => Player
            ? Player.position + (Vector2)Player.transform.TransformVector(offset)
            : transform.position + transform.TransformVector(offset);
        private Vector2 laserDirection => (worldPoint - laserCenter).normalized;
        private Vector2 laserOrigin => laserCenter + laserDirection * nearPlane;
        
        private void HandlePrimaryFire(bool value)
        {
            primaryFire = value;
        }
        
        private void HandleScreenPoint(Vector2 value)
        {
            screenPoint = value;
        }
        
        private void EvaluateLaserPoints(ICollection<Vector3> points, float deltaTime)
        {
            points.Clear();
            
            var isValidPoint = Vector2.Distance(worldPoint, laserCenter) > nearPlane;

            if (!isValidPoint) return;
            if (!primaryFire) return;
            
            var origin = laserOrigin;
            var direction = laserDirection;
            var distance = laserDistance - nearPlane;
            
            points.Add(origin);

            const int limit = 100;
            for (var i = 0; i < limit; i++)
            {
                var hit = Physics2D.Raycast(origin, direction, distance, layerMask);
                
                if (hit)
                {
                    points.Add(hit.point);
                    
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
                    points.Add(origin + direction * distance);
                    
                    return;
                }
            }
        }

        private void BuildLine(IReadOnlyList<Vector3> points)
        {
            if (!line) return;

            line.startWidth = laserWidth;
            line.endWidth = laserWidth;
            
            line.enabled = points.Count > 1;

            line.positionCount = points.Count;
            for (var i = 0; i < points.Count; i++)
            {
                line.SetPosition(i, points[i]);
            }
        }
        
        protected override void OnSetPlayer()
        {
            _playerInput = Player.GetPlayerInput();
            
            _playerInput.PrimaryFireEvent += HandlePrimaryFire;
            _playerInput.ScreenPointEvent += HandleScreenPoint;
        }
        
        private void OnDestroy()
        {
            if (_playerInput)
            {
                _playerInput.PrimaryFireEvent -= HandlePrimaryFire;
                _playerInput.ScreenPointEvent -= HandleScreenPoint;
            }
        }
        
        private void OnEnable()
        {
            if (line)
            {
                line.enabled = false;
            }
        }
        
        private void FixedUpdate()
        {
            EvaluateLaserPoints(laserPoints, Time.fixedDeltaTime);
            
            BuildLine(laserPoints);
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
    }
}