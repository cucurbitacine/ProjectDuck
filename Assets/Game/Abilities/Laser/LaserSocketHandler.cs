using UnityEngine;
using UnityEngine.Events;

namespace Game.Abilities.Laser
{
    public class LaserSocketHandler : MonoBehaviour
    {
        [SerializeField] private UnityEvent<bool> onSocketChanged = new UnityEvent<bool>(); 
        
        [Space]
        [SerializeField] private LaserSocket socket;

        private void HandleSocket(bool isOn)
        {
            onSocketChanged.Invoke(isOn);
        }
        
        private void OnEnable()
        {
            socket.OnStateChanged += HandleSocket;
        }

        private void OnDisable()
        {
            socket.OnStateChanged -= HandleSocket;
        }

        private void Start()
        {
            HandleSocket(socket.isOn);
        }
    }
}