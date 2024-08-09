using System.Collections.Generic;
using Game.InteractionSystem;
using Inputs;
using UnityEngine;

namespace Game.Player
{
    public class InteractController : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask = 1;
        
        [Space]
        [SerializeField] private PlayerController player;
        
        private PlayerInput _playerInput;
        private ContactFilter2D _filter2D = default;
        private int _countColliders;
        private readonly List<Collider2D> _colliders = new List<Collider2D>();
        
        private readonly HashSet<IFocused> _activeFocusedSet = new HashSet<IFocused>();
        private readonly HashSet<IFocused> _foundFocusedSet = new HashSet<IFocused>();
        
        private void HandleInteract(bool value)
        {
            if (!value) return;
            
            for (var i = 0; i < _countColliders; i++)
            {
                var cld2d = _colliders[i];

                if (cld2d && cld2d.TryGetComponent<IInteraction>(out var interaction))
                {
                    interaction.Interact();
                }
            }
        }

        [SerializeField] private Vector2 offsetCheckBox = Vector2.zero;
        [SerializeField] private Vector2 sizeCheckBox = Vector2.one;

        [SerializeField] private float direction = 1f;
        
        private void Overlap()
        {
            if (player == null) return;

            _filter2D.useLayerMask = true;
            _filter2D.layerMask = layerMask;
            _filter2D.useTriggers = true;

            var movement2d = player.GetMovement2D();

            if (movement2d)
            {
                if (movement2d.move.x > 0)
                {
                    direction = 1f;
                }
                else if (movement2d.move.x < 0)
                {
                    direction = -1f;
                }
            }

            var offset = Vector2.Scale(offsetCheckBox, Vector2.up + Vector2.right * direction);
            var checkPosition = player.position + (Vector2)player.transform.TransformVector(offset);

            _countColliders = Physics2D.OverlapBox(checkPosition, sizeCheckBox, 0f, _filter2D, _colliders);
            //_countColliders = Physics2D.OverlapCircle(player.position, radiusAccess, _filter2D, _colliders);
            
            _foundFocusedSet.Clear();
            for (var i = 0; i < _countColliders; i++)
            {
                var cld2d = _colliders[i];

                if (!cld2d.TryGetComponent<IFocused>(out var focused)) continue;
                
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
            Overlap();
        }

        private void OnDrawGizmos()
        {
            if (player == null) player = GetComponent<PlayerController>();

            if (Mathf.Approximately(direction, 0f))
            {
                direction = 1f;
            }
            
            //Gizmos.DrawWireSphere(player ? player.position : transform.position, radiusAccess);
            var offset = Vector2.Scale(offsetCheckBox, Vector2.up + Vector2.right * direction);
            var checkPosition = player.position + (Vector2)player.transform.TransformVector(offset);
            Gizmos.DrawWireCube(checkPosition, sizeCheckBox);
        }
    }
}