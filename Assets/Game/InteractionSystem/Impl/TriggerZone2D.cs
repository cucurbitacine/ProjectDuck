using System.Collections.Generic;
using CucuTools;
using Game.Utils;
using UnityEngine;

namespace Game.InteractionSystem.Impl
{
    public class TriggerZone2D : ToggleBase
    {
        [Header("Trigger Zone")]
        [SerializeField] private bool onlyOnce = false;
        [SerializeField] private LayerMask layerMask = 1;
        [SerializeField] private List<string> whiteList = new List<string>();

        //[Space]
        //[SerializeField] private UnityEvent<Collider2D> enterEvent = new UnityEvent<Collider2D>();
        //[SerializeField] private UnityEvent<Collider2D> exitEvent = new UnityEvent<Collider2D>();
            
        private readonly HashSet<Collider2D> colliderSet = new HashSet<Collider2D>();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.layer.Contains(layerMask)) return;

            if (whiteList.Count > 0)
            {
                if (!other.TryGet<IKeyHolder>(out var holder)) return;

                if (!whiteList.Contains(holder.Key)) return;
            }
            
            if (!colliderSet.Add(other)) return;

            if (colliderSet.Count == 1)
            {
                TurnOn(true);
            }
            
            //enterEvent.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!colliderSet.Remove(other)) return;

            if (colliderSet.Count == 0)
            {
                if (!onlyOnce)
                {
                    TurnOn(false);
                }
            }
            
            //exitEvent.Invoke(other);
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