using UnityEngine;
using UnityEngine.Events;

namespace Game.InteractionSystem.Utils
{
    [RequireComponent(typeof(ISwitchable))]
    public sealed class SwitchEvent : MonoBehaviour
    {
        [SerializeField] private bool inverse = false;
        [SerializeField] private UnityEvent<bool> switchEvent = new UnityEvent<bool>();

        private ISwitchable _switcher;

        private void HandleSwitch(bool value)
        {
            switchEvent.Invoke(inverse ? !value : value);
        }
        
        private void Awake()
        {
            _switcher = GetComponent<ISwitchable>();
        }

        private void OnEnable()
        {
            if (_switcher != null)
            {
                _switcher.OnChanged += HandleSwitch;
            }
        }

        private void OnDisable()
        {
            if (_switcher != null)
            {
                _switcher.OnChanged -= HandleSwitch;
            }
        }

        private void Start()
        {
            if (_switcher != null)
            {
                HandleSwitch(_switcher.TurnedOn);
            }
        }
    }
}