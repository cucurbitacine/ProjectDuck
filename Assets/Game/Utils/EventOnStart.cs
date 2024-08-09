using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Utils
{
    public class EventOnStart : MonoBehaviour
    {
        [SerializeField] private UnityEvent eventOnStart = new UnityEvent();

        private void Start()
        {
            eventOnStart.Invoke();
        }
    }
}
