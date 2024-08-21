using Game.Abilities;
using Game.Abilities.Electricity;
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
        
        public Vector2 position => _movementController ? _movementController.position : transform.position;
        
        public bool PickAbility(PickupAbility pickupAbility)
        {
            if (activeAbility && pickupAbility.AbilityId >= 0)
            {
                if (pickupAbility.AbilityId == activeAbility.AbilityId)
                {
                    return false;
                }
            }
            
            DropAbility();
            
            var abilityPrefab = pickupAbility.GetAbilityPrefab();
            activeAbility = Instantiate(abilityPrefab);
            activeAbility.SetPlayer(this);

            return true;
        }

        public void DropAbility()
        {
            if (activeAbility)
            {
                activeAbility.Drop();
                
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

        public AbilityBase GetAbility()
        {
            return activeAbility;
        }
        
        private void HandleModelLoad(GameObject model)
        {
            var handles = model.GetComponentsInChildren<IPlayerHandle>();

            foreach (var handle in handles)
            {
                handle?.SetPlayer(this);
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
            _movementController.SetPlayerCollider(_playerCollider);
            
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