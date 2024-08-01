using System;

namespace Game.InteractionSystem
{
    public interface ISwitchable
    {
        public bool TurnedOn { get; }

        public event Action<bool> OnChanged;
    }
}