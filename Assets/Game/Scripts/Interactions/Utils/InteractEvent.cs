using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Interactions.Utils
{
    [RequireComponent(typeof(IInteractable))]
    public sealed class InteractEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<GameObject> interactEvent = new UnityEvent<GameObject>();

        private IInteractable _interactable;

        private void HandleInteract(GameObject actor)
        {
            interactEvent.Invoke(actor);
        }

        private void Awake()
        {
            _interactable = GetComponent<IInteractable>();
        }

        private void OnEnable()
        {
            _interactable.OnInteracted += HandleInteract;
        }

        private void OnDisable()
        {
            _interactable.OnInteracted -= HandleInteract;
        }
    }
}