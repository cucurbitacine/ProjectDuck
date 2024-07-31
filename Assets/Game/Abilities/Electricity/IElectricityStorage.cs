using System;

namespace Game.Abilities.Electricity
{
    public interface IElectricityStorage
    {
        public event Action<int> OnChargeChanged;
        
        public int ElectricityCharge { get; }

        public int TryPick(int amount);
        
        public int TryPut(int amount);
    }
}