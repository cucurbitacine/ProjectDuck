using System;
using Game.Scripts.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Interactions.Impl
{
    public class PlateController : MonoBehaviour, IToggle, IPaused
    {
        [SerializeField] private TriggerZone2D pressZone;

        [Header("Events")]
        [SerializeField] private UnityEvent onTurnedOn = new UnityEvent();
        [SerializeField] private UnityEvent onTurnedOff = new UnityEvent();
        
        public bool Paused => pressZone.Paused;
        public bool TurnedOn => pressZone.TurnedOn;
        
        public event Action<bool> OnValueChanged;
        
        private void HandlePressZone(bool value)
        {
            OnValueChanged?.Invoke(value);

            if (value)
            {
                onTurnedOn.Invoke();
            }
            else
            {
                onTurnedOff.Invoke();
            }
        }
        
        public void TurnOn(bool value)
        {
            pressZone.TurnOn(value);
        }
        
        public void Pause(bool value)
        {
            pressZone.Pause(value);
        }
        
        private void OnEnable()
        {
            pressZone.OnValueChanged += HandlePressZone;
        }
        
        private void OnDisable()
        {
            pressZone.OnValueChanged -= HandlePressZone;
        }
    }
}