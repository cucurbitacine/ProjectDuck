using System;

namespace Game.Scripts.Interactions
{
    public interface IToggle
    {
        public bool TurnedOn { get; }

        public event Action<bool> OnValueChanged;

        public void TurnOn(bool value);
    }
}