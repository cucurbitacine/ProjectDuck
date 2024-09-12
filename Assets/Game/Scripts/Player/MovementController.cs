using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Movements;
using Inputs;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Movement2D))]
    public class MovementController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool paused = false;
        [SerializeField] private float platformIgnoreDuration = 1f;
        [SerializeField] private bool jumpOnlyOnGround = false;
        
        private Movement2D _movement2d;
        private PlayerInput _playerActions;
        private Collider2D _playerCollider;
        
        private bool _lastJump;
        private bool _down;
        private bool _lastDown;
        
        private Vector2 move { get; set; }
        private HashSet<Collider2D> ignoredPlatforms { get; } = new HashSet<Collider2D>();
        
        public Vector2 position => _movement2d ? _movement2d.position : transform.position;
        
        public Movement2D GetMovement2D()
        {
            return _movement2d;
        }

        public void SetPlayerActions(PlayerInput playerInput)
        {
            if (_playerActions)
            {
                _playerActions.MoveEvent -= HandleMove;
                _playerActions.JumpEvent -= HandleJump;
            }

            _playerActions = playerInput;
            
            if (_playerActions)
            {
                _playerActions.MoveEvent += HandleMove;
                _playerActions.JumpEvent += HandleJump;
            }
        }
        
        public void SetPlayerCollider(Collider2D cld2d)
        {
            _playerCollider = cld2d;
            
            _movement2d.Ground2D.SetWidth(_playerCollider.bounds.size.x);
            _movement2d.Ground2D.BoxCasting = _playerCollider is BoxCollider2D;
        }
        
        public void Pause(bool value)
        {
            paused = value;

            if (paused)
            {
                move = Vector2.zero;
            }
        }
        
        private void HandleMove(Vector2 newMove)
        {
            if (paused) return;
            
            move = newMove;

            _lastDown = _down;
            _down = move.y < -0.5f;

            if (!_lastDown && _down)
            {
                Down();
            }
        }
 
        private void HandleJump(bool jump)
        {
            if (paused) return;
            
            if (!_lastJump && jump)
            {
                if (!jumpOnlyOnGround || _movement2d.isGrounded)
                {
                    _movement2d.Jump();
                }
            }

            _lastJump = jump;
        }
        
        private void UpdateMovement()
        {
            _movement2d.Move(move);
        }

        private bool IsPlatform(Collider2D other)
        {
            return other.TryGetComponent<PlatformEffector2D>(out _);
        }
        
        private bool ShouldIgnore(Collider2D other)
        {
            if (!IsPlatform(other)) return false;

            if (_movement2d.isGrounded)
            {
                return _movement2d.Ground2D.groundCollider != other;
            }
            
            return true;
        }
        
        private void Down()
        {
            StartCoroutine(IgnorePlatform(platformIgnoreDuration));
        }
        
        private IEnumerator IgnorePlatform(float duration)
        {
            if (_movement2d.isGrounded && IsPlatform(_movement2d.Ground2D.groundCollider))
            {
                var platform = _movement2d.Ground2D.groundCollider;

                if (ignoredPlatforms.Add(platform))
                {
                    Ignore(platform, true);
                    Physics2D.IgnoreCollision(_playerCollider, platform, true);

                    yield return new WaitForSeconds(duration);
                
                    Physics2D.IgnoreCollision(_playerCollider, platform, false);
                    Ignore(platform, false);
                
                    ignoredPlatforms.Remove(platform);
                }
            }
        }
        
        private void Ignore(Collider2D cld, bool value)
        {
            _movement2d.Ground2D.Ignore(cld, value);
        }

        private void Awake()
        {
            _movement2d = GetComponent<Movement2D>();
        }

        private void OnEnable()
        {
            if (_playerActions)
            {
                _playerActions.MoveEvent += HandleMove;
                _playerActions.JumpEvent += HandleJump;
            }
        }

        private void OnDisable()
        {
            if (_playerActions)
            {
                _playerActions.MoveEvent -= HandleMove;
                _playerActions.JumpEvent -= HandleJump;
            }
        }

        private void Update()
        {
            UpdateMovement();
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            _movement2d.Ground2D.CheckGround();
            
            if (ShouldIgnore(other.collider))
            {
                Ignore(other.collider, true);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!ignoredPlatforms.Contains(other.collider))
            {
                Ignore(other.collider, false);
            }
        }
    }
}
