using System;
using UnityEngine;

namespace Game.Scripts.Player
{
    [DisallowMultipleComponent]
    public class StepHandler : MonoBehaviour
    {
        public event Action OnStep;

        public void HandleStep()
        {
            OnStep?.Invoke();
        }
    }
}