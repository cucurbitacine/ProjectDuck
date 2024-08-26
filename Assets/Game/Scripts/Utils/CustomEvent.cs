using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Utils
{
    public sealed class CustomEvent : MonoBehaviour
    {
        [SerializeField] private EventMoment moment = EventMoment.Start;
        
        [Space]
        [SerializeField] private UnityEvent unityEvent = new UnityEvent();

        private void InvokeAs(EventMoment eventMoment)
        {
            if (eventMoment == moment)
            {
                unityEvent.Invoke();
            }
        }
        
        private void Awake()
        {
            InvokeAs(EventMoment.Awake);
        }
        
        private void OnEnable()
        {
            InvokeAs(EventMoment.OnEnable);
        }
        
        private void Start()
        {
            InvokeAs(EventMoment.Start);
        }

        private void Update()
        {
            InvokeAs(EventMoment.Update);
        }
        
        private void LateUpdate()
        {
            InvokeAs(EventMoment.LateUpdate);
        }
        
        private void FixedUpdate()
        {
            InvokeAs(EventMoment.FixedUpdate);
        }
        
        private void OnDisable()
        {
            InvokeAs(EventMoment.OnDisable);
        }
        
        private void OnDestroy()
        {
            InvokeAs(EventMoment.OnDestroy);
        }
        
        public enum EventMoment
        {
            Awake,
            
            OnEnable,
            Start,
            
            Update,
            LateUpdate,
            FixedUpdate,
            
            OnDisable,
            OnDestroy,
        }
    }
}
