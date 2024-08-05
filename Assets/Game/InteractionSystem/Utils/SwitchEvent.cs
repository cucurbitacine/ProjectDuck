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
            _switcher.OnChanged += HandleSwitch;
        }

        private void OnDisable()
        {
            _switcher.OnChanged -= HandleSwitch;
        }

        private void Start()
        {
            HandleSwitch(_switcher.TurnedOn);
        }
    }
}