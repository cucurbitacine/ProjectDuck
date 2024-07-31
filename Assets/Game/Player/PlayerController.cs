using Game.Abilities;
using Game.Movements;
using Game.Utils;
using Inputs;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private AbilityBase activeAbility;
        
        [Header("References")]
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private ModelLoader modelLoader;
        [SerializeField] private MovementController movementController;

        private Collider2D _playerCollider;
        
        public Vector2 position => movementController ? movementController.position : transform.position;
        
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
            return movementController ? movementController.GetMovement() : null;
        }
        
        public Bounds GetBounds()
        {
            return _playerCollider ? _playerCollider.bounds : default;
        }
        
        private void HandleModelLoad(GameObject model)
        {
            _playerCollider = model.GetComponent<Collider2D>();
            
            SetupMovementController(model);
        }

        private void SetupMovementController(GameObject root)
        {
            if (!movementController) return;
            
            movementController.SetPlayerCollider(_playerCollider);
            
            var movement = movementController.GetMovement();
            
            if (!movement) return;
            
            var handles = root.GetComponentsInChildren<IMovement2DHandle>();

            foreach (var handle in handles)
            {
                handle?.SetMovement2D(movement);
            }
        }
        
        private void Awake()
        {
            if (modelLoader == null) modelLoader = GetComponent<ModelLoader>();
            if (movementController == null) movementController = GetComponent<MovementController>();
        }

        private void OnEnable()
        {
            modelLoader.OnModelLoaded += HandleModelLoad;

            HandleModelLoad(modelLoader.GetModel());
        }

        private void OnDisable()
        {
            modelLoader.OnModelLoaded -= HandleModelLoad;
        }

        private void Start()
        {
            movementController.SetPlayerActions(playerInput);
        }
    }
}