using System;
using System.Collections.Generic;
using Game.Scripts.Interactions;
using Inputs;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class InteractorController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask layerMask = 1;
        [SerializeField] [Min(0f)] private float timeout = 0.5f;
        [Space]
        [SerializeField] private Vector2 offsetCheckBox = Vector2.zero;
        [SerializeField] private Vector2 sizeCheckBox = Vector2.one;
        
        [Header("References")]
        [SerializeField] private PlayerController player;
        
        private float _direction = 1f;

        private float _time;
        
        private PlayerInput _playerInput;
        private ContactFilter2D _filter2D = default;
        private int _countColliders;
        private readonly List<Collider2D> _colliders = new List<Collider2D>();
        
        private readonly HashSet<IInteraction> _activeFocusedSet = new HashSet<IInteraction>();
        private readonly HashSet<IInteraction> _foundFocusedSet = new HashSet<IInteraction>();

        public event Action OnInteracted; 
        
        private void HandleInteract(bool value)
        {
            if (!value) return;

            if (_time > 0f) return;
            
            _time = timeout;
            
            for (var i = 0; i < _countColliders; i++)
            {
                var cld2d = _colliders[i];

                if (cld2d && cld2d.TryGetComponent<IInteraction>(out var interaction))
                {
                    interaction.Interact(player.gameObject);
                }
            }
            
            OnInteracted?.Invoke();
        }
        
        private void Overlap(float deltaTime)
        {
            if (player == null) return;

            if (_time > 0f)
            {
                _time -= deltaTime;
            }
            
            _filter2D.useLayerMask = true;
            _filter2D.layerMask = layerMask;
            _filter2D.useTriggers = true;

            var movement2d = player.GetMovement2D();

            if (movement2d)
            {
                if (movement2d.move.x > 0)
                {
                    _direction = 1f;
                }
                else if (movement2d.move.x < 0)
                {
                    _direction = -1f;
                }
            }

            var offset = Vector2.Scale(offsetCheckBox, Vector2.up + Vector2.right * _direction);
            var checkPosition = player.position + (Vector2)player.transform.TransformVector(offset);

            _countColliders = Physics2D.OverlapBox(checkPosition, sizeCheckBox, 0f, _filter2D, _colliders);
            //_countColliders = Physics2D.OverlapCircle(player.position, radiusAccess, _filter2D, _colliders);
            
            _foundFocusedSet.Clear();
            for (var i = 0; i < _countColliders; i++)
            {
                var cld2d = _colliders[i];

                if (!cld2d.TryGetComponent<IInteraction>(out var focused)) continue;
                
                // if found item is new, set focus true
                if (_foundFocusedSet.Add(focused) && !_activeFocusedSet.Contains(focused))
                {
                    focused.Focus(true);
                }
            }

            // if along
            _activeFocusedSet.RemoveWhere(t =>
            {
                if (_foundFocusedSet.Contains(t)) return false;
                
                t.Focus(false);
                return true;
            });

            foreach (var focused in _foundFocusedSet)
            {
                _activeFocusedSet.Add(focused);
            }
        }
        
        private void Awake()
        {
            if (player == null) player = GetComponent<PlayerController>();
        }

        private void OnEnable()
        {
            if (player)
            {
                _playerInput = player.GetPlayerInput();

                _playerInput.InteractEvent += HandleInteract;
            }
        }
        
        private void OnDisable()
        {
            if (_playerInput)
            {
                _playerInput.InteractEvent -= HandleInteract;
            }
        }
        
        private void FixedUpdate()
        {
            Overlap(Time.fixedDeltaTime);
        }

        private void OnDrawGizmos()
        {
            if (player == null) player = GetComponent<PlayerController>();

            if (Mathf.Approximately(_direction, 0f))
            {
                _direction = 1f;
            }
            
            //Gizmos.DrawWireSphere(player ? player.position : transform.position, radiusAccess);
            var offset = Vector2.Scale(offsetCheckBox, Vector2.up + Vector2.right * _direction);
            var checkPosition = player.position + (Vector2)player.transform.TransformVector(offset);
            Gizmos.DrawWireCube(checkPosition, sizeCheckBox);
        }
    }
}