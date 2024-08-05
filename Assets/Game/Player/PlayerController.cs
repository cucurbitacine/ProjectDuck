using Game.Abilities;
using Game.Combat;
using Game.Core;
using Game.Movements;
using Game.Utils;
using Inputs;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(ModelLoader))]
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private AbilityBase activeAbility;
        
        [Header("References")]
        [SerializeField] private PlayerInput playerInput;
        
        private ModelLoader _modelLoader;
        private MovementController _movementController;
        private Collider2D _playerCollider;
        
        public Vector2 position => _movementController ? _movementController.position : transform.position;
        
        public Health Health { get; private set; }
        
        public void PickAbility(PickupAbility pickupAbility)
        {
            DropAbility();
            
            var abilityPrefab = pickupAbility.GetAbility();
            activeAbility = Instantiate(abilityPrefab);
            activeAbility.SetPlayer(this);
        }

        public void DropAbility()
        {
            if (activeAbility)
            {
                Destroy(activeAbility.gameObject);

                activeAbility = null;
            }
        }

        public PlayerInput GetPlayerInput()
        {
            return playerInput;
        }

        public Movement2D GetMovement()
        {
            return _movementController.GetMovement();
        }
        
        public Bounds GetBounds()
        {
            return _playerCollider ? _playerCollider.bounds : default;
        }
        
        public void Pause(bool value)
        {
            _movementController.Pause(value);
        }
        
        private void HandleModelLoad(GameObject model)
        {
            _playerCollider = model.GetComponent<Collider2D>();
            
            SetupMovementController(model);
        }

        private void SetupMovementController(GameObject root)
        {
            _movementController.SetPlayerCollider(_playerCollider);
            
            var movement = _movementController.GetMovement();
            
            if (!movement) return;
            
            var handles = root.GetComponentsInChildren<IMovement2DHandle>();

            foreach (var handle in handles)
            {
                handle?.SetMovement2D(movement);
            }
        }
        
        private void Awake()
        {
            _modelLoader = GetComponent<ModelLoader>();
            _movementController = GetComponent<MovementController>();
            Health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _modelLoader.OnModelLoaded += HandleModelLoad;

            HandleModelLoad(_modelLoader.GetModel());
            
            GameManager.Instance.SetPlayer(gameObject);
        }

        private void OnDisable()
        {
            _modelLoader.OnModelLoaded -= HandleModelLoad;
            
            GameManager.Instance.RemovePlayer();
        }

        private void Start()
        {
            if (playerInput)
            {
                _movementController.SetPlayerActions(playerInput);
            }
            else
            {
                Debug.LogError($"\"{name} ({GetType().Name})\" has no \"{nameof(PlayerInput)}\"");
            }
        }
    }
}