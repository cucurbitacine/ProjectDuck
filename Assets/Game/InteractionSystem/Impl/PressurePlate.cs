using UnityEngine;

namespace Game.InteractionSystem.Impl
{
    public class PressurePlate : ToggleBase
    {
        [SerializeField] private TriggerZone2D triggerZone;

        private void HandleTriggerZone(bool value)
        {
            TurnOn(value);
        }
        
        private void OnEnable()
        {
            triggerZone.OnValueChanged += HandleTriggerZone;
        }
        
        private void OnDisable()
        {
            triggerZone.OnValueChanged -= HandleTriggerZone;
        }
    }
}