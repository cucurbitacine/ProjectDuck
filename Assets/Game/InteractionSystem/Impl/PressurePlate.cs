using UnityEngine;

namespace Game.InteractionSystem.Impl
{
    public class PressurePlate : SwitcherBase
    {
        [SerializeField] private TriggerZone2D triggerZone;

        private void HandleTriggerZone(bool value)
        {
            TurnOn(value);
        }
        
        private void OnEnable()
        {
            triggerZone.OnChanged += HandleTriggerZone;
        }
        
        private void OnDisable()
        {
            triggerZone.OnChanged -= HandleTriggerZone;
        }
    }
}