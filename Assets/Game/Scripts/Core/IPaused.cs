namespace Game.Scripts.Core
{
    public interface IPaused
    {
        public bool Paused { get; }
        
        public void Pause(bool value);
    }
}