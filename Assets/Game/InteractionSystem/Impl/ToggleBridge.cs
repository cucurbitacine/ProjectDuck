using System;
using Game.Utils;
using UnityEngine;

namespace Game.InteractionSystem.Impl
{
    public class ToggleBridge : MonoBehaviour, IToggle, IPaused
    {
        [SerializeField] private ToggleBase toggle;

        public bool Paused => toggle.Paused;
        public bool TurnedOn => toggle.TurnedOn;
        
        public event Action<bool> OnValueChanged;
        
        private void HandleToggle(bool value)
        {
            OnValueChanged?.Invoke(value);
        }
        
        public void TurnOn(bool value)
        {
            toggle.TurnOn(value);
        }
        
        public void Pause(bool value)
        {
            toggle.Pause(value);
        }
        
        private void OnEnable()
        {
            toggle.OnValueChanged += HandleToggle;
        }
        
        private void OnDisable()
        {
            toggle.OnValueChanged -= HandleToggle;
        }
    }
}