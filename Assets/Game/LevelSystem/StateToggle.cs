using CucuTools.StateMachines;
using Game.InteractionSystem;
using UnityEngine;

namespace Game.LevelSystem
{
    public class StateToggle : StateBase
    {
        [Header("Switcher")]
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