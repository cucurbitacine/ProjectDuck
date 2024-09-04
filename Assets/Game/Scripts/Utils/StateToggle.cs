using CucuTools.StateMachines;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Utils
{
    public class StateToggle : StateBase
    {
        [Header("Toggle")]
        [SerializeField] private bool expectedValue = true;
        [SerializeField] private ToggleBase _toggle;

        protected override void OnUpdateState(float deltaTime)
        {
            if (_toggle.TurnedOn == expectedValue)
            {
                isDone = true;
            }
        }
    }
}