using System;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public bool Focused { get; private set; }
        [field: SerializeField] public bool Paused { get; private set; }

        [Space]
        [SerializeField] private bool onlyOnce = false;
        
        public event Action<bool> OnFocusChanged;
        public event Action<GameObject> OnInteracted;

        public void Focus(bool value)
        {
            if (Paused) return;
            
            if (Focused == value) return;

            Focused = value;

            OnFocusChanged?.Invoke(Focused);
        }

        public void Interact(GameObject actor)
        {
            if (Paused) return;
            
            OnInteracted?.Invoke(actor);
            
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

        [ContextMenu(nameof(Interact))]
        private void Interact()
        {
            Interact(gameObject);
        }
    }
}