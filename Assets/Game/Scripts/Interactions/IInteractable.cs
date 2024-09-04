using System;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public interface IInteractable : IFocused
    {
        public event Action<GameObject> OnInteracted; 
        
        public void Interact(GameObject actor);
    }
}