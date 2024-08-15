using UnityEngine;
using UnityEngine.Events;

namespace Game.InteractionSystem
{
    public class InteractableButton : InteractableBase
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