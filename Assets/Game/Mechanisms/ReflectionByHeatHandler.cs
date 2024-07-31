using UnityEngine;

namespace Game.Mechanisms
{
    [DisallowMultipleComponent]
    public class ReflectionByHeatHandler : MonoBehaviour
    {
        [Min(0)]
        [SerializeField] private float heatMin = 0;
        [Min(0)]
        [SerializeField] private float heatMax = 4;
        
        [Space]
        [SerializeField] private ReflectorSurface reflector;
        [SerializeField] private HeatedSurface heated;
        
        private void HandleHeat(float heat)
        {
            reflector.IsReflector = heatMin <= heat && heat <= heatMax;
        }
        
        private void OnEnable()
        {
            heated.OnHeatChanged += HandleHeat;
            
            HandleHeat(heated.Heat);
        }

        
        private void OnDisable()
        {
            heated.OnHeatChanged -= HandleHeat;
        }
    }
}