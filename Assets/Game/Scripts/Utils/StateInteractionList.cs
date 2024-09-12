using System;
using System.Collections.Generic;
using System.Linq;
using CucuTools.StateMachines;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Utils
{
    public class StateInteractionList : StateBase
    {
        [Header("Interactables")]
        [SerializeField] private List<InteractableBase> interactables = new List<InteractableBase>();

        private readonly List<InteractionListener> _listeners = new List<InteractionListener>();
        
        protected override void OnStartState()
        {
            interactables.RemoveAll(i => i == null);

            if (interactables.Count > 0)
            {
                _listeners.AddRange(interactables.Select(i => new InteractionListener(i)));
            
                foreach (var listener in _listeners)
                {
                    listener.OnInteracted += CheckState;
                }
            }
            else
            {
                isDone = true;
            }
        }
        
        protected override void OnStopState()
        {
            foreach (var listener in _listeners)
            {
                listener.OnInteracted -= CheckState;
            }
            
            _listeners.Clear();
        }

        private void CheckState()
        {
            if (_listeners.All(i => i.Interacted))
            {
                isDone = true;
            }
        }
        
        private sealed class InteractionListener : IDisposable
        {
            public bool Interacted { get; private set; }

            public event Action OnInteracted;
            
            private readonly IInteractable _interactable;

            public InteractionListener(IInteractable interactable)
            {
                _interactable = interactable;
                
                _interactable.OnInteracted += HandleInteraction;
            }
            
            public void Dispose()
            {
                _interactable.OnInteracted -= HandleInteraction;
            }
            
            private void HandleInteraction(GameObject actor)
            {
                if (Interacted) return;
                
                Interacted = true;
                
                OnInteracted?.Invoke();
            }
        }
    }
}