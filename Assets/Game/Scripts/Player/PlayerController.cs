using Game.Scripts.Combat;
using Game.Scripts.Movements;
using Inputs;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(ModelLoader))]
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject activeAbility;
        
        [Header("References")]
        [SerializeField] private PlayerInput playerInput;
        
        private Collider2D _playerCollider;
        private MovementController _movementController;
        
        public Health Health { get; private set; }
        public ModelLoader ModelLoader { get; private set; }
        
        public Vector2 position => _movementController ? _movementController.position : transform.position;

        public void SetAbility(GameObject newAbility)
        {
            DropAbility();

            activeAbility = newAbility;
        }
        
        public GameObject GetAbility()
        {
            return activeAbility;
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
            var handles = model.GetComponentsInChildren<IPlayerHandler>();

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
        }

        private void OnDisable()
        {
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