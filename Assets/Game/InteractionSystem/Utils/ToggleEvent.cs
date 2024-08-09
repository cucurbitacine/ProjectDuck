using UnityEngine;
using UnityEngine.Events;

namespace Game.InteractionSystem.Utils
{
    [RequireComponent(typeof(IToggle))]
    public sealed class ToggleEvent : MonoBehaviour
    {
        [SerializeField] private bool inverse = false;
        [SerializeField] private UnityEvent<bool> toggleValueChanged = new UnityEvent<bool>();

        private IToggle _switcher;

        private void HandleToggle(bool value)
        {
            toggleValueChanged.Invoke(inverse ? !value : value);
        }

        private void Awake()
        {
            _switcher = GetComponent<IToggle>();
        }

        private void OnEnable()
        {
            _switcher.OnValueChanged += HandleToggle;
        }

        private void OnDisable()
        {
            _switcher.OnValueChanged -= HandleToggle;
        }

        private void Start()
        {
            HandleToggle(_switcher.TurnedOn);
        }
    }
}