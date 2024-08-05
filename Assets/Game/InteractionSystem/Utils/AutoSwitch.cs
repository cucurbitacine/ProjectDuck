using System.Collections;
using UnityEngine;

namespace Game.InteractionSystem.Utils
{
    [RequireComponent(typeof(ISwitchable))]
    public sealed class AutoSwitch : MonoBehaviour
    {
        [SerializeField] private bool expectedValue = false;
        [Min(0f)]
        [SerializeField] private float timeout = 1f;
        
        private ISwitchable _switcher;
        private Coroutine _switchingProcess;
        
        private void HandleSwitch(bool value)
        {
            if (_switchingProcess != null) StopCoroutine(_switchingProcess);
            
            if (expectedValue != value)
            {
                _switchingProcess = StartCoroutine(Switching());
            }
        }

        private IEnumerator Switching()
        {
            yield return new WaitForSeconds(timeout);

            _switcher?.TurnOn(expectedValue);
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