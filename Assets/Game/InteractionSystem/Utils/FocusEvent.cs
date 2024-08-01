using UnityEngine;
using UnityEngine.Events;

namespace Game.InteractionSystem.Utils
{
    [RequireComponent(typeof(IFocused))]
    public sealed class FocusEvent : MonoBehaviour
    {
        [SerializeField] private bool inverse = false;
        [SerializeField] private UnityEvent<bool> focusEvent = new UnityEvent<bool>();

        private IFocused _focused;

        private void HandleFocus(bool value)
        {
            focusEvent.Invoke(inverse ? !value : value);
        }
        
        private void Awake()
        {
            _focused = GetComponent<IFocused>();
        }

        private void OnEnable()
        {
            if (_focused != null)
            {
                _focused.OnFocusChanged += HandleFocus;
            }
        }

        private void OnDisable()
        {
            if (_focused != null)
            {
                _focused.OnFocusChanged -= HandleFocus;
            }
        }

        private void Start()
        {
            if (_focused != null)
            {
                HandleFocus(_focused.Focused);
            }
        }
    }
}