using UnityEngine;

namespace Game.InteractionSystem.Impl
{
    public class DoorController : SwitcherBase
    {
        [Space]
        [SerializeField] private GameObject door;
        [SerializeField] private InteractableBase interactable;

        private void OpenDoor(bool value)
        {
            door.SetActive(!value);
        }
        
        private void HandleInteract()
        {
            TurnOn(!TurnedOn);
        }

        private void HandleFocus(bool value)
        {
            //
        }
        
        private void OnEnable()
        {
            interactable.OnFocusChanged += HandleFocus;
            interactable.OnInteracted += HandleInteract;
            
            OnChanged += OpenDoor;
        }

        private void OnDisable()
        {
            interactable.OnFocusChanged -= HandleFocus;
            interactable.OnInteracted -= HandleInteract;
            
            OnChanged -= OpenDoor;
        }

        private void Start()
        {
            OpenDoor(TurnedOn);
        }
    }
}