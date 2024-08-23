using CucuTools.StateMachines;
using Game.Interactions;
using UnityEngine;

namespace Game.Utils
{
    public class StateInteraction : StateBase
    {
        [Header("Interactable")]
        [SerializeField] private InteractableBase interactable;

        private void HandleInteraction(GameObject actor)
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