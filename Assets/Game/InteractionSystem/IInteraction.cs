using System;

namespace Game.InteractionSystem
{
    public interface IInteraction
    {
        public event Action OnInteracted; 
        
        public void Interact();
    }
}