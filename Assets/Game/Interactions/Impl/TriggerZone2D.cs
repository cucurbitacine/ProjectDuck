using System.Collections.Generic;
using CucuTools;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Interactions.Impl
{
    public class TriggerZone2D : ToggleBase
    {
        [Header("Trigger Zone")]
        [SerializeField] private bool onlyOnce = false;
        [SerializeField] private LayerMask layerMask = 1;

        [Space]
        [SerializeField] private UnityEvent<Collider2D> onEntered = new UnityEvent<Collider2D>();
        [SerializeField] private UnityEvent<Collider2D> onExited = new UnityEvent<Collider2D>();
            
        private readonly HashSet<Collider2D> colliderSet = new HashSet<Collider2D>();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (onlyOnce && TurnedOn) return;
            
            if (!other.gameObject.layer.Contains(layerMask)) return;

            if (!colliderSet.Add(other)) return;

            if (colliderSet.Count == 1)
            {
                TurnOn(true);
            }
            
            onEntered.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (onlyOnce && TurnedOn) return;
            
            if (!colliderSet.Remove(other)) return;

            if (colliderSet.Count == 0)
            {
                TurnOn(false);
            }

            onExited.Invoke(other);
        }

        private void OnValidate()
        {
            if (TryGetComponent<Collider2D>(out var cld2d))
            {
                cld2d.isTrigger = true;
            }
        }
    }
}