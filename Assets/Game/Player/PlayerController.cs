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
        [SerializeField] private PlayerActionsProfile playerActions;
        [SerializeField] private ModelLoader modelLoader;
        [SerializeField] private MovementController movementController;

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

        public PlayerActionsProfile GetPlayerActions()
        {
            return playerActions;
        }
        
        private void HandleModelLoad(GameObject model)
        {
            var playerCollider = model.GetComponent<Collider2D>();
            movementController.SetPlayerCollider(playerCollider);

            SetMovement2D(model);
        }

        private void SetMovement2D(GameObject root)
        {
            var movement = movementController?.GetMovement();
            
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
            movementController.SetPlayerActions(playerActions);
        }
    }
}