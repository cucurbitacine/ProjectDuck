using System;
using UnityEngine;

namespace Game.InteractionSystem.Impl
{
    [DisallowMultipleComponent]
    public sealed class KeyHolder : MonoBehaviour, IKeyHolder
    {
        [SerializeField] private string key = Guid.NewGuid().ToString();

        public string Key
        {
            get => key;
            set => key = value;
        }
    }
}