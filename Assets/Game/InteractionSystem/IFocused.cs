using System;

namespace Game.InteractionSystem
{
    public interface IFocused
    {
        public bool Focused { get; }
        
        public event Action<bool> OnFocusChanged; 
        
        public void Focus(bool value);
    }
}