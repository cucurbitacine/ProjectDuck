using System;
using UnityEngine;

namespace Game.Mechanisms
{
    public class SocketBase : MonoBehaviour
    {
        [SerializeField] private bool _isOn = false;
        
        public event Action<bool> OnSocketChanged; 
        
        public bool isOn
        {
            get => _isOn;
            protected set
            {
                if (_isOn == value) return;
                _isOn = value;
                OnSocketChanged?.Invoke(_isOn);
            }
        }
    }
}