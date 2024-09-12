using System;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public sealed class FocusedBase : MonoBehaviour, IFocused
    {
        [field: SerializeField] public bool Focused { get; private set; }
        [field: SerializeField] public bool Paused { get; private set; }
        
        public event Action<bool> OnFocusChanged;
        
        public void Focus(bool value)
        {
            if (Paused) return;
            
            if (Focused == value) return;
            
            Focused = value;
            
            OnFocusChanged?.Invoke(Focused);
        }

        public void Pause(bool value)
        {
            if (Focused && value)
            {
                Focus(false);
            }

            Paused = value;
        }
    }
}