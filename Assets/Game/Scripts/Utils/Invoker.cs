using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Utils
{
    public sealed class Invoker : MonoBehaviour
    {
        [SerializeField] private InvokeMoment moment = InvokeMoment.Start;
        [SerializeField] [Min(0f)] private float delay = 0f;
        
        [Space]
        [SerializeField] private UnityEvent unityEvent = new UnityEvent();

        private void InvokeUnityEvent()
        {
            unityEvent.Invoke();
        }
        
        private void InvokeDelayed(float time)
        {
            if (time > 0f)
            {
                Invoke(nameof(InvokeUnityEvent), time);
            }
            else
            {
                InvokeUnityEvent();
            }
        }
        
        private void InvokeAs(InvokeMoment eventMoment)
        {
            if (eventMoment == moment)
            {
                InvokeDelayed(delay);
            }
        }
        
        private void Awake()
        {
            InvokeAs(InvokeMoment.Awake);
        }
        
        private void OnEnable()
        {
            InvokeAs(InvokeMoment.OnEnable);
        }
        
        private void Start()
        {
            InvokeAs(InvokeMoment.Start);
        }

        private void OnDisable()
        {
            InvokeAs(InvokeMoment.OnDisable);
        }
        
        private void OnDestroy()
        {
            InvokeAs(InvokeMoment.OnDestroy);
        }
        
        public enum InvokeMoment
        {
            Awake,
            OnEnable,
            Start,
            OnDisable,
            OnDestroy,
        }
    }
}
