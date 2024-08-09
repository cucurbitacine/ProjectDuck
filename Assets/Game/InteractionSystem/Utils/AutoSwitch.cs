using System.Collections;
using UnityEngine;

namespace Game.InteractionSystem.Utils
{
    [RequireComponent(typeof(IToggle))]
    public sealed class AutoSwitch : MonoBehaviour
    {
        [SerializeField] private bool expectedValue = false;
        [Min(0f)]
        [SerializeField] private float timeout = 1f;
        
        private IToggle _switcher;
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
            _switcher = GetComponent<IToggle>();
        }

        private void OnEnable()
        {
            _switcher.OnValueChanged += HandleSwitch;
        }

        private void OnDisable()
        {
            _switcher.OnValueChanged -= HandleSwitch;
        }

        private void Start()
        {
            HandleSwitch(_switcher.TurnedOn);
        }
    }
}