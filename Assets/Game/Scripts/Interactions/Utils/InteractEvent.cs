using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Interactions.Utils
{
    [RequireComponent(typeof(IInteraction))]
    public sealed class InteractEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<GameObject> interactEvent = new UnityEvent<GameObject>();

        private IInteraction _interaction;

        private void HandleInteract(GameObject actor)
        {
            interactEvent.Invoke(actor);
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