using UnityEngine;

namespace Game.Scripts.Abilities.Laser
{
    public interface ILaserHandler
    {
        public bool IsReflector { get; }
        
        public void Impact(Vector2 point, float value);
    }
}