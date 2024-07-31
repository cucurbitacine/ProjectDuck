using System;
using Inputs;
using UnityEngine;

namespace Game.Abilities.Electricity
{
    public class ElectricityAbility : AbilityBase, IElectricityStorage
    {
        [Header("Settings")]
        [SerializeField] private float radiusPickup = 0.5f;
        [SerializeField] private LayerMask layerMask = 1;
        
        [Header("Info")]
        [Min(0)]
        [SerializeField] private int electricityCharge = 0;
        
        [Header("Input")]
        [SerializeField] private bool primaryFire;
        [SerializeField] private bool secondaryFire;
        [SerializeField] private Vector2 screenPoint;
        
        private PlayerInput _playerInput;

        private static Camera CameraMain => Camera.main;
        
        private Vector2 worldPoint => _playerInput ? CameraMain.ScreenToWorldPoint(screenPoint) : transform.position;

        public event Action<int> OnChargeChanged; 
        
        public int ElectricityCharge
        {
            get => electricityCharge;
            private set
            {
                if (electricityCharge != value)
                {
                    electricityCharge = value;
                    OnChargeChanged?.Invoke(electricityCharge);
                }
            }
        }

        public int TryPick(int amount)
        {
            amount = Math.Min(amount, ElectricityCharge);

            ElectricityCharge -= amount;

            return amount;
        }

        public int TryPut(int amount)
        {
            ElectricityCharge += amount;

            return amount;
        }
        
        private void HandlePrimaryFire(bool value)
        {
            primaryFire = value;
            
            if (primaryFire)
            {
                var cld2d = Physics2D.OverlapCircle(worldPoint, radiusPickup, layerMask);

                if (cld2d && cld2d.TryGetComponent<IElectricityStorage>(out var storage))
                {
                    TryPut(storage.TryPick(1));
                }
            }
        }
        
        private void HandleSecondaryFire(bool value)
        {
            secondaryFire = value;
            
            if (secondaryFire)
            {
                var cld2d = Physics2D.OverlapCircle(worldPoint, radiusPickup, layerMask);

                if (cld2d && cld2d.TryGetComponent<IElectricityStorage>(out var storage))
                {
                    storage.TryPut(TryPick(1));
                }
            }
        }
        
        private void HandleScreenPoint(Vector2 value)
        {
            screenPoint = value;
        }
        
        protected override void OnSetPlayer()
        {
            _playerInput = Player.GetPlayerInput();
            
            _playerInput.PrimaryFireEvent += HandlePrimaryFire;
            _playerInput.SecondaryFireEvent += HandleSecondaryFire;
            _playerInput.ScreenPointEvent += HandleScreenPoint;
        }
        
        private void OnDestroy()
        {
            if (_playerInput)
            {
                _playerInput.PrimaryFireEvent -= HandlePrimaryFire;
                _playerInput.SecondaryFireEvent -= HandleSecondaryFire;
                _playerInput.ScreenPointEvent -= HandleScreenPoint;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = primaryFire || secondaryFire ? Color.white : Color.grey;
            Gizmos.DrawWireSphere(worldPoint, radiusPickup);
        }
    }
}