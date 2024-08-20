using System;
using UnityEngine;

namespace Game.InteractionSystem
{
    public interface IInteraction : IFocused
    {
        public event Action<GameObject> OnInteracted; 
        
        public void Interact(GameObject actor);
    }
}