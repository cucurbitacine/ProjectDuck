using Inputs;
using UnityEngine;

namespace Game.Abilities.Teleportation
{
    public class TeleportationAbility : AbilityBase
    {
        [Header("Settings")]
        [Min(0)]
        [SerializeField] private float timeout = 1f;
        
        [Header("Input")]
        [SerializeField] private bool primaryFire;
        [SerializeField] private Vector2 screenPoint;
        
        private PlayerInput _playerInput;
        private Bounds _playerBounds;
        private float _lastTimeTeleportation;
        
        private static Camera CameraMain => Camera.main;
        
        private Vector2 worldPoint => _playerInput ? CameraMain.ScreenToWorldPoint(screenPoint) : transform.position;

        private bool CanTeleport()
        {
            return timeout > 0 && Time.time - _lastTimeTeleportation > timeout;
        }
        
        private bool IsValidWorldPosition(Vector2 teleportPosition)
        {
            _playerBounds = Player.GetBounds();

            return !Physics2D.OverlapBox(teleportPosition, _playerBounds.size, 0f);
        }
        
        private Vector2 GetValidTeleportPosition(Vector2 teleportPosition)
        {
            return teleportPosition + Vector2.down * _playerBounds.size.y * 0.5f;
        }
        
        private void Teleport(Vector2 teleportPosition)
        {
            var movement = Player.GetMovement();
                
            movement.Warp(teleportPosition);

            _lastTimeTeleportation = Time.time;
        }
        
        private void HandlePrimaryFire(bool value)
        {
            primaryFire = value;

            if (!primaryFire)
            {
                if (CanTeleport() && IsValidWorldPosition(worldPoint))
                {
                    var teleportPosition = GetValidTeleportPosition(worldPoint);
                    
                    Teleport(teleportPosition);
                }
            }
        }
        
        private void HandleScreenPoint(Vector2 value)
        {
            screenPoint = value;
        }
        
        protected override void OnSetPlayer()
        {
            _lastTimeTeleportation = float.MinValue;
            
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

        private void OnDrawGizmos()
        {
            Gizmos.color = CanTeleport() ? (IsValidWorldPosition(worldPoint) ? Color.green : Color.yellow) : Color.red;
            Gizmos.DrawWireCube(worldPoint, _playerBounds.size);
        }
    }
}