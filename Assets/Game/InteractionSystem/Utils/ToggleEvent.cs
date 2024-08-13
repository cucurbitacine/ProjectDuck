using UnityEngine;
using UnityEngine.Events;

namespace Game.InteractionSystem.Utils
{
    [RequireComponent(typeof(IToggle))]
    public sealed class ToggleEvent : MonoBehaviour
    {
        [SerializeField] private bool inverse = false;
        [SerializeField] private UnityEvent<bool> toggleValueChanged = new UnityEvent<bool>();
        [Space]
        [SerializeField] private UnityEvent turnedOn = new UnityEvent();
        [SerializeField] private UnityEvent turnedOff = new UnityEvent();
        
        private IToggle _switcher;

        private void HandleToggle(bool value)
        {
            toggleValueChanged.Invoke(inverse ? !value : value);

            if (inverse)
            {
                if (value)
                {
                    turnedOff.Invoke();
                }
                else
                {
                    turnedOn.Invoke();
                } 
            }
            else
            {
                if (value)
                {
                    turnedOn.Invoke();
                }
                else
                {
                    turnedOff.Invoke();
                }
            }
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