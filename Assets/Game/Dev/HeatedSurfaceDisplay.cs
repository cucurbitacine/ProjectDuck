using Game.Mechanisms;
using UnityEngine;

namespace Game.Dev
{
    public class HeatedSurfaceDisplay : MonoBehaviour
    {
        [Min(0)]
        [SerializeField] private float heatMax = 10;
        
        [Space]
        [SerializeField] private Color minHeatColor = Color.blue;
        [SerializeField] private Color maxHeatColor = Color.red;

        [Space]
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private HeatedSurface heated;

        private void HandleHeat(float heat)
        {
            sprite.color = Color.LerpUnclamped(minHeatColor, maxHeatColor, heatMax > 0 ? heat / heatMax : 0f);
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
