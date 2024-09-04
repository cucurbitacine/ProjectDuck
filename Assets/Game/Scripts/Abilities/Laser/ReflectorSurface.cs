using UnityEngine;

namespace Game.Scripts.Abilities.Laser
{
    [DisallowMultipleComponent]
    public sealed class ReflectorSurface : MonoBehaviour, ILaserHandler
    {
        [SerializeField] private bool isReflector = true;
        
        public bool IsReflector
        {
            get => isReflector;
            set => isReflector = value;
        }

        public void Impact(Vector2 point, float value)
        {
        }
    }
}