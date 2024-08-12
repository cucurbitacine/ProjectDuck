using CucuTools.StateMachines;
using Game.InteractionSystem;
using UnityEngine;

namespace Game.LevelSystem
{
    public class StateInteract : StateBase
    {
        [Header("Interactable")]
        [SerializeField] private InteractableBase interactable;

        private void HandleInteraction()
        {
            isDone = true;
        }
        
        protected override void OnStartState()
        {
            interactable.OnInteracted += HandleInteraction;
        }
        
        protected override void OnStopState()
        {
            interactable.OnInteracted -= HandleInteraction;
        }
    }
}