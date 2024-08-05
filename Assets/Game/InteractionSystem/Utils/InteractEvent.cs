using UnityEngine;
using UnityEngine.Events;

namespace Game.InteractionSystem.Utils
{
    [RequireComponent(typeof(IInteraction))]
    public sealed class InteractEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent interactEvent = new UnityEvent();

        private IInteraction _interaction;

        private void HandleInteract()
        {
            interactEvent.Invoke();
        }

        private void Awake()
        {
            _interaction = GetComponent<IInteraction>();
        }

        private void OnEnable()
        {
            _interaction.OnInteracted += HandleInteract;
        }

        private void OnDisable()
        {
            _interaction.OnInteracted -= HandleInteract;
        }
    }
}