using System;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public class ToggleBase : MonoBehaviour, IToggle, IPaused
    {
        [field: SerializeField] public bool TurnedOn { get; private set; }
        [field: SerializeField] public bool Paused { get; private set; }
        
        public event Action<bool> OnValueChanged;
        
        public void TurnOn(bool value)
        {
            if (Paused) return;
            
            if (TurnedOn == value) return;
            
            TurnedOn = value;
            
            OnValueChanged?.Invoke(value);
        }
        
        public virtual void Pause(bool value)
        {
            Paused = value;
        }
    }
}