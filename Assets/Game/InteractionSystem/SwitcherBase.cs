using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.InteractionSystem
{
    public class SwitcherBase : MonoBehaviour, ISwitchable
    {
        [field: SerializeField] public bool TurnedOn { get; private set; }
        [field: SerializeField] public bool Paused { get; private set; }
        
        public event Action<bool> OnChanged;
        
        public void TurnOn(bool value)
        {
            if (Paused) return;
            
            if (TurnedOn == value) return;
            
            TurnedOn = value;
            
            OnChanged?.Invoke(value);
        }
        
        public void Pause(bool value)
        {
            Paused = value;
        }
    }
}