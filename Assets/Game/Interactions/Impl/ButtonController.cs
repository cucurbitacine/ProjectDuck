using UnityEngine;
using UnityEngine.Events;

namespace Game.Interactions.Impl
{
    public class ButtonController : InteractableBase
    {
        [Header("Button")]
        [SerializeField] private UnityEvent onClicked = new UnityEvent();

        private void HandleInteraction(GameObject actor)
        {
            onClicked.Invoke();
        }
        
        private void OnEnable()
        {
            OnInteracted += HandleInteraction;
        }
        
        private void OnDisable()
        {
            OnInteracted -= HandleInteraction;
        }
    }
}