using CucuTools.DamageSystem;
using Game.Abilities;
using Game.Combat;
using Game.LevelSystem;
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
        
        private Collider2D _playerCollider;
        private MovementController _movementController;
        
        public Health Health { get; private set; }
        public ModelLoader ModelLoader { get; private set; }
        public DamageReceiver DamageReceiver => Health?.DamageReceiver;
        
        public Vector2 position => _movementController ? _movementController.position : transform.position;
        
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

        public Movement2D GetMovement2D()
        {
            return _movementController.GetMovement2D();
        }
        
        public Bounds GetBounds()
        {
            return _playerCollider ? _playerCollider.bounds : default;
        }
        
        public void Pause(bool value)
        {
            if (Health.IsDead) return;
            
            _movementController.Pause(value);
        }
        
        private void HandleModelLoad(GameObject model)
        {
            SetupMovementController(model);
        }

        private void SetupMovementController(GameObject root)
        {
            _movementController.SetPlayerCollider(_playerCollider);
            
            var movement = _movementController.GetMovement2D();
            
            if (!movement) return;
            
            var handles = root.GetComponentsInChildren<IMovement2DHandle>();

            foreach (var handle in handles)
            {
                handle?.SetMovement2D(movement);
            }
        }
        
        private void HandleDeath()
        {
            _movementController.Pause(true);
        }
        
        private void Awake()
        {
            _playerCollider = GetComponent<Collider2D>();
            
            ModelLoader = GetComponent<ModelLoader>();
            _movementController = GetComponent<MovementController>();
            Health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            ModelLoader.OnModelLoaded += HandleModelLoad;
            
            Health.OnDied += HandleDeath;
            
            LevelManager.SetPlayer(this);
        }

        private void OnDisable()
        {
            LevelManager.RemovePlayer();
            
            ModelLoader.OnModelLoaded -= HandleModelLoad;
            
            Health.OnDied -= HandleDeath;
        }

        private void Start()
        {
            HandleModelLoad(ModelLoader.GetModel());
            
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