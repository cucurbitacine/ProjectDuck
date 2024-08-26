using System.Collections.Generic;
using Game.Scripts.SFX;
using Inputs;
using UnityEngine;

namespace Game.Scripts.Abilities.Teleportation
{
    public class TeleportationAbility : AbilityBase
    {
        [Header("Settings")]
        [Min(0f)] [SerializeField] private float maxDistance = 8f;
        [Min(0f)] [SerializeField] private float timeout = 1f;
        [SerializeField] private LayerMask obstacleLayerMask = 1;

        [Header("SFX")]
        [SerializeField] private SoundFX aimSfx;
        [SerializeField] private SoundFX teleportSfx;
        
        [Header("VFX")]
        [Min(0)] [SerializeField] private int minEmissionRateOverTime = 1000;
        [Min(0)] [SerializeField] private int maxEmissionRateOverTime = 4000;
        [SerializeField] private ParticleSystem teleportEffect;
        
        [Space]
        [SerializeField] private GameObject teleportHitEffectPrefab;
        
        [Header("Input")]
        [SerializeField] private bool primaryFire;
        
        private PlayerInput _playerInput;
        private Bounds _playerBounds;
        private float _lastTimeTeleportation;
        private ContactFilter2D _filter = new ContactFilter2D();
        
        private readonly List<Collider2D> overlap = new List<Collider2D>();
        
        private Vector2 worldPoint => _playerInput ? _playerInput.WorldPoint : transform.position;

        public void Teleport(Vector2 teleportPosition)
        {
            var movement2d = Player.GetMovement2D();
                
            movement2d.Warp(teleportPosition);

            _lastTimeTeleportation = Time.time;
            
            if (teleportEffect && teleportEffect.isPlaying)
            {
                teleportEffect.gameObject.SetActive(false);
            }
            
            if (teleportHitEffectPrefab)
            {
                Instantiate(teleportHitEffectPrefab, Player.position, Quaternion.identity);
                Instantiate(teleportHitEffectPrefab, teleportPosition, Quaternion.identity);
            }

            if (teleportSfx)
            {
                teleportSfx.Play();
            }
        }
        
        private bool CanTeleport()
        {
            return timeout > 0 && Time.time - _lastTimeTeleportation > timeout;
        }
        
        private bool IsValidWorldPosition(Vector2 teleportPosition)
        {
            var playerPosition = Player ? Player.position : (Vector2)transform.position;
            if (Vector2.Distance(playerPosition, teleportPosition) > maxDistance)
            {
                return false;
            }
            
            _playerBounds = Player ? Player.GetBounds() : default;
            
            _filter.useTriggers = true;
            _filter.useLayerMask = true;
            _filter.layerMask = obstacleLayerMask;

            var count = Physics2D.OverlapBox(teleportPosition, _playerBounds.size, 0f, _filter, overlap);

            return count == 0;
        }
        
        private Vector2 GetValidTeleportPosition(Vector2 teleportPosition)
        {
            return teleportPosition + Vector2.down * (_playerBounds.size.y * 0.5f);
        }
        
        private void HandlePrimaryFire(bool value)
        {
            primaryFire = value;
            
            if (primaryFire) return;
            
            if (CanTeleport() && IsValidWorldPosition(worldPoint))
            {
                var teleportPosition = GetValidTeleportPosition(worldPoint);
                    
                Teleport(teleportPosition);
            }
        }

        private void UpdateEffect()
        {
            if (!teleportEffect) return;
            
            if (!Player) return;
            
            var teleportReady = CanTeleport() && IsValidWorldPosition(worldPoint);

            if (teleportReady)
            {
                teleportEffect.gameObject.SetActive(true);
                teleportEffect.Play();
            }
            else
            {
                teleportEffect.Stop();
            }
            
            var model = Player.ModelLoader.GetModel();
            if (model.TryGetComponent<SpriteRenderer>(out var playerSprite))
            {
                var shape = teleportEffect.shape;
                        
                shape.shapeType = ParticleSystemShapeType.SpriteRenderer;
                shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
                shape.spriteRenderer = playerSprite;
                shape.texture = playerSprite.sprite.texture;
                        
                shape.position = teleportEffect.transform.InverseTransformPoint(GetValidTeleportPosition(worldPoint));
                if (Player.GetMovement2D().move.x > 0f)
                {
                    shape.scale = new Vector3(1f, 1f, 1f);
                }
                else if (Player.GetMovement2D().move.x < 0f)
                {
                    shape.scale = new Vector3(-1f, 1f, 1f);
                }
            }

            var emission = teleportEffect.emission;
            emission.rateOverTime = primaryFire && teleportReady ? maxEmissionRateOverTime : minEmissionRateOverTime;
            
            Cursor.visible = !(primaryFire && teleportReady);

            if (aimSfx)
            {
                var shouldPlay = primaryFire && teleportReady;

                aimSfx.Play(shouldPlay);
            }
        }
        
        protected override void OnSetPlayer()
        {
            _lastTimeTeleportation = float.MinValue;
            
            _playerInput = Player.GetPlayerInput();
            
            _playerInput.PrimaryFireEvent += HandlePrimaryFire;
        }

        private void OnDestroy()
        {
            if (_playerInput)
            {
                _playerInput.PrimaryFireEvent -= HandlePrimaryFire;
            }
            
            Cursor.visible = true;
        }
        
        private void Update()
        {
            UpdateEffect();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(Player ? Player.position : (Vector2)transform.position, maxDistance);
            Gizmos.color = CanTeleport() ? (IsValidWorldPosition(worldPoint) ? Color.green : Color.yellow) : Color.red;
            Gizmos.DrawWireCube(worldPoint, _playerBounds.size);
        }
    }
}