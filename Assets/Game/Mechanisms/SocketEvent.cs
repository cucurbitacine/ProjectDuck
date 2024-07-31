using UnityEngine;
using UnityEngine.Events;

namespace Game.Mechanisms
{
    public class SocketEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<bool> onSocketChanged = new UnityEvent<bool>(); 
        
        [Space]
        [SerializeField] private SocketBase socket;

        private void HandleSocket(bool isOn)
        {
            onSocketChanged.Invoke(isOn);
        }
        
        private void OnEnable()
        {
            socket.OnSocketChanged += HandleSocket;
        }

        private void OnDisable()
        {
            socket.OnSocketChanged -= HandleSocket;
        }

        private void Start()
        {
            HandleSocket(socket.isOn);
        }
    }
}