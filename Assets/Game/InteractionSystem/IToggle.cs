using System;

namespace Game.InteractionSystem
{
    public interface IToggle
    {
        public bool TurnedOn { get; }

        public event Action<bool> OnValueChanged;

        public void TurnOn(bool value);
    }
}