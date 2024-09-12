using CucuTools.StateMachines;
using UnityEngine;

namespace Game.Scripts.Utils
{
    public class StateWait : StateBase
    {
        [Header("Wait")]
        [Min(0f)] [SerializeField] private float waitTime = 1f;
        [Min(0f)] [SerializeField] private float timeSpread = 0f;

        private float _randomDelayTime = 0f;

        protected override void OnStartState()
        {
            _randomDelayTime = Random.value * timeSpread;
        }

        protected override void OnUpdateState(float deltaTime)
        {
            if (time > waitTime + _randomDelayTime)
            {
                isDone = true;
            }
        }
    }
}