using System;
using System.Collections;
using UnityEngine;

namespace Game.InteractionSystem.Utils
{
    [RequireComponent(typeof(SwitcherBase))]
    public sealed class AutoSwitch : MonoBehaviour
    {
        [SerializeField] private bool expectedValue = false;
        [Min(0f)]
        [SerializeField] private float timeout = 1f;
        
        private SwitcherBase _switcher;
        private Coroutine _switching;
        
        private void HandleSwitch(bool value)
        {
            if (_switching != null) StopCoroutine(_switching);
            
            if (expectedValue != value)
            {
                _switching = StartCoroutine(Switching());
            }
        }

        private IEnumerator Switching()
        {
            yield return new WaitForSeconds(timeout);

            if (_switcher)
            {
                _switcher.TurnOn(expectedValue);
            }
        }

        private void Awake()
        {
            _switcher = GetComponent<SwitcherBase>();
        }

        private void OnEnable()
        {
            if (_switcher)
            {
                _switcher.OnChanged += HandleSwitch;
            }
        }

        private void OnDisable()
        {
            if (_switcher)
            {
                _switcher.OnChanged -= HandleSwitch;
            }
        }

        private void Start()
        {
            if (_switcher)
            {
                HandleSwitch(_switcher.TurnedOn);
            }
        }
    }
}