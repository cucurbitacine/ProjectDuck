using System;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.Movements
{
    [DisallowMultipleComponent]
    public class Ground2D : MonoBehaviour
    {
        #region SerializeField

        [Min(0f)]
        [SerializeField] private float width = 1f;
        [Min(0f)]
        [SerializeField] private float distance = 0.1f;
        [SerializeField] private LayerMask groundLayer = 1;

        [Space] 
        [Range(0f, 90f)]
        [SerializeField] private float maxSlopeAngle = 45f;

        #endregion

        #region Private Fields & Properties

        private bool _wasGrounded;
        private ContactFilter2D _groundFilter = default;
        
        private readonly List<RaycastHit2D> _hits = new List<RaycastHit2D>();
        private readonly HashSet<Collider2D> _ignore = new HashSet<Collider2D>();
        
        private GroundHit2D groundHit { get; set; }
        
        private Vector2 boxRaycastOrigin => pointGroundCheck - Direction.normalized * boxRaycastDistance * 0.5f;
        private float boxRaycastWidth => width;
        private float boxRaycastDistance => distance;
        private Vector2 boxRaycastSize => new Vector2(boxRaycastWidth, boxRaycastDistance);
        private float boxRaycastAngle => Vector2.SignedAngle(Vector2.up, Direction);

        private Vector2 circleRaycastOrigin => pointGroundCheck - Direction.normalized * circleRaycastRadius;
        private float circleRaycastRadius => width * 0.5f;
        private float circleRaycastDistance => distance;

        #endregion

        #region Static

        private static readonly Dictionary<Rigidbody2D, Inertial2D> InertialDict = new Dictionary<Rigidbody2D, Inertial2D>();
        
        public static bool AddInertial(Rigidbody2D rigidbody2d, Inertial2D inertial2D)
        {
            return InertialDict.TryAdd(rigidbody2d, inertial2D);
        }
        
        public static bool RemoveInertial(Rigidbody2D rigidbody2d, out Inertial2D inertial2D)
        {
            return InertialDict.Remove(rigidbody2d, out inertial2D);
        }

        #endregion
        
        #region Public API

        [field: Space]
        [field: SerializeField] public bool BoxCasting { get; set; } = true;
        [field: SerializeField] public Vector2 Direction { get; set; } = Vector2.down;
        
        public bool isGrounded => onSurface && Mathf.Abs(groundSlopeAngle) <= maxSlopeAngle && !_ignore.Contains(groundCollider);
        public bool onSurface => groundHit;
        public bool onSlope => !Mathf.Approximately(Mathf.Abs(groundSlopeAngle), 0f);
        public bool isInertial => groundRigidbody && InertialDict.TryGetValue(groundRigidbody, out var inertial) && inertial.active;
        
        public Vector2 pointGroundCheck => transform.position;
        public Vector2 groundPoint => groundHit.raycastHit ? groundHit.raycastHit.point : pointGroundCheck;
        public Vector2 groundNormal => groundHit.raycastHit ? groundHit.raycastHit.normal : -Direction;
        public Rigidbody2D groundRigidbody => groundHit.raycastHit.rigidbody;
        public Collider2D groundCollider => groundHit.raycastHit.collider;
        public Vector2 groundVelocity => groundRigidbody
            ? groundRigidbody.GetPointVelocity(groundPoint)
            : Vector2.zero;
        
        public float groundSlopeAngle => Vector2.SignedAngle(groundNormal, -Direction);
        
        public event Action<bool> OnGrounded;
        
        public void Ignore(Collider2D cld, bool value)
        {
            if (value)
            {
                if (isGrounded && groundCollider == cld)
                {
                    groundHit = default;
                    
                    HandleEvent();
                }
                
                if (_ignore.Add(cld))
                {
                    //Debug.Log($"Start Ignore {cld.name}");
                }
            }
            else
            {
                if (_ignore.Remove(cld))
                {
                    //Debug.Log($"Stop Ignore {cld.name}");
                }
            }
        }

        public void SetWidth(float newWidth)
        {
            width = newWidth;
        }
        
        public void CheckGround()
        {
            var count = Raycast(_hits);

            groundHit = count > 0 ? (GroundHit2D)_hits[0] : default;
            
            /*
            // Search the closest
            var minDistance = float.MaxValue;
            for (var i = 0; i < count; i++)
            {
                var hit = _hits[i];

                if (hit.distance >= minDistance) continue;
                
                minDistance = hit.distance;
                groundHit = (GroundHit2D)hit;
            }
            */

            if (groundHit && _ignore.Contains(groundHit.raycastHit.collider))
            {
                groundHit = default;
            }

            HandleEvent();
        }

        #endregion

        #region Private API

        private int Raycast(List<RaycastHit2D> hits)
        {
            _groundFilter.useLayerMask = true;
            _groundFilter.layerMask = groundLayer;
            _groundFilter.useTriggers = false;
            
            return BoxCasting ? BoxCast(_groundFilter, hits) : CircleCast(_groundFilter, hits);
        }
        
        private int BoxCast(ContactFilter2D filter2d, List<RaycastHit2D> hits)
        {
            return Physics2D.BoxCast(boxRaycastOrigin, boxRaycastSize, boxRaycastAngle, Direction, filter2d, hits, boxRaycastDistance);
        }

        private int CircleCast(ContactFilter2D filter2d, List<RaycastHit2D> hits)
        {
            var count = Physics2D.CircleCast(circleRaycastOrigin, circleRaycastRadius, Direction.normalized, filter2d, hits, circleRaycastDistance);

            if (count <= 0) return count;
            
            var projectOrigin = Vector3.Project(circleRaycastOrigin, Direction);

            var i = 0;
            while (i < count)
            {
                var hit = hits[i];

                var projectPoint = Vector3.Project(hit.point, Direction);
                var projectDirection = projectPoint - projectOrigin;

                if (Vector2.Dot(Direction, projectDirection) < 0) 
                {
                    hits.RemoveAt(i);
                    
                    count--;
                }
                else
                {
                    i++;
                }
            }

            return count;
        }
        
        private void HandleEvent()
        {
            if ((!_wasGrounded && isGrounded) || (_wasGrounded && !isGrounded))
            {
                OnGrounded?.Invoke(isGrounded);
            }

            _wasGrounded = isGrounded;
        }

        #endregion

        #region MonoBehaviour

        private void OnDrawGizmos()
        {
            if (onSurface)
            {
                Gizmos.color = isGrounded ? Color.green : Color.yellow;
                Gizmos.DrawSphere(groundPoint, 0.05f);
                
                Gizmos.color = new Color(0.75f, 0.75f, 0.75f, 0.75f);
                if (BoxCasting)
                {
                    Tools.DrawBox(boxRaycastOrigin + Direction.normalized * groundHit.raycastHit.distance, boxRaycastSize, boxRaycastAngle);
                }
                else
                {
                    Gizmos.DrawWireSphere(groundHit.raycastHit.point + groundHit.raycastHit.normal * circleRaycastRadius, circleRaycastRadius);
                }
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(groundHit.raycastHit.point, groundHit.raycastHit.point + groundHit.raycastHit.normal * distance);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(groundPoint, 0.05f);
                
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                if (BoxCasting)
                {
                    Tools.DrawBox(boxRaycastOrigin + Direction.normalized * boxRaycastDistance, boxRaycastSize, boxRaycastAngle);
                }
                else
                {
                    Gizmos.DrawWireSphere(circleRaycastOrigin + Direction.normalized * circleRaycastDistance, circleRaycastRadius);
                }
            }
        }

        #endregion
    }

    [Serializable]
    public struct GroundHit2D
    {
        public RaycastHit2D raycastHit;
        
        public static implicit operator bool(GroundHit2D hit) => hit.raycastHit;
        public static explicit operator GroundHit2D(RaycastHit2D hit) => new GroundHit2D() { raycastHit = hit };
    }
}