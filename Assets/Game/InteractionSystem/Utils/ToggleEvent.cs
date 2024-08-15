using UnityEngine;
using UnityEngine.Events;

namespace Game.InteractionSystem.Utils
{
    [RequireComponent(typeof(IToggle))]
    public sealed class ToggleEvent : MonoBehaviour
    {
        [SerializeField] private bool inverse = false;
        [SerializeField] private UnityEvent<bool> toggleValueChanged = new UnityEvent<bool>();
        
        private IToggle _toggle;

        private void HandleToggle(bool value)
        {
            toggleValueChanged.Invoke(inverse ? !value : value);
        }

        private void Awake()
        {
            _toggle = GetComponent<IToggle>();
        }

        private void OnEnable()
        {
            _toggle.OnValueChanged += HandleToggle;
        }

        private void OnDisable()
        {
            _toggle.OnValueChanged -= HandleToggle;
        }

        private void Start()
        {
            HandleToggle(_toggle.TurnedOn);
        }
    }
}