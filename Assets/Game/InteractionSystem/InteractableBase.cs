using System;
using UnityEngine;

namespace Game.InteractionSystem
{
    public class InteractableBase : MonoBehaviour, IFocused, IInteraction
    {
        [field: SerializeField] public bool Focused { get; private set; }
        [field: SerializeField] public bool Paused { get; private set; }

        [Space]
        [SerializeField] private bool onlyOnce = false;
        
        public event Action<bool> OnFocusChanged;
        public event Action OnInteracted;

        public void Focus(bool value)
        {
            if (Paused) return;
            
            if (Focused == value) return;

            Focused = value;

            OnFocusChanged?.Invoke(Focused);
        }

        public void Interact()
        {
            if (Paused) return;
            
            OnInteracted?.Invoke();

            if (onlyOnce)
            {
                Pause(true);
            }
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