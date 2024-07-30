using UnityEngine;

namespace Game.Abilities.Laser
{
    public interface ILaserHandler
    {
        public bool IsReflector { get; }
        
        public void Impact(Vector2 point, float value);
    }
}