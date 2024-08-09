using System;
using Game.Abilities.Electricity;
using UnityEngine;

namespace Game.InteractionSystem.Impl
{
    public class ElectricitySocket : ToggleBase, IElectricityStorage
    {
        [SerializeField] private int electricityCharge = 0;
        [SerializeField] private int electricityChargeMax = 1;
        
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
                    
                    TurnOn(ElectricityCharge >= electricityChargeMax);
                }
            }
        }

        public int TryPick(int amount)
        {
            amount = Mathf.Min(amount, ElectricityCharge);
            
            ElectricityCharge -= amount;

            return amount;
        }

        public int TryPut(int amount)
        {
            var available = electricityChargeMax - ElectricityCharge;
            
            amount = Mathf.Min(amount, available);
            
            ElectricityCharge += amount;
            
            return amount;
        }

        private void Start()
        {
            TurnOn(ElectricityCharge >= electricityChargeMax);
        }
    }
}