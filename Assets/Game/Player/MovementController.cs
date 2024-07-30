using System.Collections;
using System.Collections.Generic;
using Game.Movements;
using Inputs;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(Movement2D))]
    public class MovementController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float platformIgnoreDuration = 1f;
        [SerializeField] private bool jumpOnlyOnGround = false;
        
        [Header("References")]
        [SerializeField] private Movement2D movement;

        private Vector2 move { get; set; }
        private HashSet<Collider2D> ignoredPlatforms { get; } = new HashSet<Collider2D>();

        private PlayerActionsProfile playerActions;
        private Collider2D playerCollider;
        
        private bool _lastJump;
        private bool _down;
        private bool _lastDown;

        public Movement2D GetMovement()
        {
            return movement;
        }

        public void SetPlayerActions(PlayerActionsProfile playerActionsProfile)
        {
            if (playerActions)
            {
                playerActions.MoveEvent -= OnMove;
                playerActions.JumpEvent -= OnJump;
            }

            playerActions = playerActionsProfile;
            
            if (playerActions)
            {
                playerActions.MoveEvent += OnMove;
                playerActions.JumpEvent += OnJump;
            }
        }
        
        public void SetPlayerCollider(Collider2D cld2d)
        {
            playerCollider = cld2d;
        }
        
        private void OnMove(Vector2 move)
        {
            this.move = move;

            _lastDown = _down;
            _down = this.move.y < -0.5f;

            if (!_lastDown && _down)
            {
                Down();
            }
        }
 
        private void OnJump(bool jump)
        {
            if (!_lastJump && jump)
            {
                if (!jumpOnlyOnGround || movement.isGrounded)
                {
                    movement.Jump();
                }
                
            }

            _lastJump = jump;
        }
        
        private void UpdateMovement()
        {
            movement.Move(move);
        }

        private bool IsPlatform(Collider2D other)
        {
            return other.TryGetComponent<PlatformEffector2D>(out _);
        }
        
        private bool ShouldIgnore(Collider2D other)
        {
            if (!IsPlatform(other)) return false;

            if (movement.isGrounded)
            {
                return movement.ground.groundCollider != other;
            }
            
            return true;
        }
        
        private void Down()
        {
            StartCoroutine(IgnorePlatform(platformIgnoreDuration));
        }
        
        private IEnumerator IgnorePlatform(float duration)
        {
            if (playerCollider && movement.isGrounded && IsPlatform(movement.ground.groundCollider))
            {
                var platform = movement.ground.groundCollider;

                if (ignoredPlatforms.Add(platform))
                {
                    Ignore(platform, true);
                    Physics2D.IgnoreCollision(playerCollider, platform, true);

                    yield return new WaitForSeconds(duration);
                
                    Physics2D.IgnoreCollision(playerCollider, platform, false);
                    Ignore(platform, false);
                
                    ignoredPlatforms.Remove(platform);
                }
            }
        }
        
        private void Ignore(Collider2D cld, bool value)
        {
            movement.ground.Ignore(cld, value);
        }
        
        private void OnEnable()
        {
            if (playerActions)
            {
                playerActions.MoveEvent += OnMove;
                playerActions.JumpEvent += OnJump;
            }
        }

        private void OnDisable()
        {
            if (playerActions)
            {
                playerActions.MoveEvent -= OnMove;
                playerActions.JumpEvent -= OnJump;
            }
        }

        private void Update()
        {
            UpdateMovement();
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            movement.ground.CheckGround();
            
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
