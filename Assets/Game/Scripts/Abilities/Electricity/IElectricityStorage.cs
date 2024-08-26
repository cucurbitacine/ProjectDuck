using System;
using Game.Scripts.Interactions;

namespace Game.Scripts.Abilities.Electricity
{
    public interface IElectricityStorage : IFocused
    {
        public event Action<int> OnChargeChanged;
        
        public int ElectricityCharge { get; }
        public int ElectricityChargeMax { get; }

        public int HowMuchAbleToSend(int amount);
        public int HowMuchAbleToReceive(int amount);
        
        public int SendCharge(int amount);
        public int ReceiveCharge(int amount);
    }
}