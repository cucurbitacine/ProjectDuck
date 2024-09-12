using System;
using Game.Scripts.Interactions;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Abilities.Electricity
{
    public class ElectricitySocket : ToggleBase, IElectricityStorage
    {
        [field: SerializeField] public bool Focused { get; private set; }
        
        [Header("Electricity")]
        [SerializeField] private int electricityCharge = 0;
        [SerializeField] private int electricityChargeMax = 1;

        [Space]
        [SerializeField] private UnityEvent onChargedFull = new UnityEvent();
        [SerializeField] private UnityEvent onChargedEmpty = new UnityEvent();
        
        public event Action<int> OnChargeChanged;
        public event Action<bool> OnFocusChanged;
        
        public int ElectricityCharge
        {
            get => electricityCharge;
            set
            {
                if (electricityCharge != value)
                {
                    electricityCharge = value;
                    
                    OnChargeChanged?.Invoke(electricityCharge);
                    
                    TurnOn(ElectricityCharge >= ElectricityChargeMax);

                    if (TurnedOn)
                    {
                        onChargedFull.Invoke();
                    }
                    else
                    {
                        onChargedEmpty.Invoke();
                    }
                }
            }
        }

        public int ElectricityChargeMax => electricityChargeMax;
        
        public void Focus(bool value)
        {
            if (Paused) return;

            Focused = value;
            
            OnFocusChanged?.Invoke(Focused);
        }

        public override void Pause(bool value)
        {
            if (value && Focused)
            {
                Focus(false);
            }
            
            base.Pause(value);
        }
        
        public int HowMuchAbleToSend(int amount)
        {
            return Mathf.Min(amount, ElectricityCharge);
        }

        public int HowMuchAbleToReceive(int amount)
        {
            var available = ElectricityChargeMax - ElectricityCharge;
            
            return Mathf.Min(amount, available);
        }

        public int SendCharge(int amount)
        {
            amount = HowMuchAbleToSend(amount);
            
            ElectricityCharge -= amount;

            return amount;
        }

        public int ReceiveCharge(int amount)
        {
            amount = HowMuchAbleToReceive(amount);
            
            ElectricityCharge += amount;
            
            return amount;
        }

        private void Start()
        {
            TurnOn(ElectricityCharge >= ElectricityChargeMax);
        }

        private void OnValidate()
        {
            electricityChargeMax = Mathf.Max(1, electricityChargeMax);
            electricityCharge = Mathf.Clamp(electricityCharge, 0, electricityChargeMax);
        }
    }
}