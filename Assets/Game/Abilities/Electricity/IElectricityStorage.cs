using System;
using Game.InteractionSystem;

namespace Game.Abilities.Electricity
{
    public interface IElectricityStorage : IFocused
    {
        public event Action<int> OnChargeChanged;
        
        public int ElectricityCharge { get; set; }

        public int HowMuchAbleToSend(int amount);
        public int HowMuchAbleToReceive(int amount);
        
        public int SendCharge(int amount);
        public int ReceiveCharge(int amount);
    }
}