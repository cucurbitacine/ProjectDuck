using System;
using UnityEngine;

namespace Game.SFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioListener))]
    public class MainAudioListener : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = Vector3.zero;
        
        public AudioListener ListenerMain { get; private set; }
        
        private void Awake()
        {
            ListenerMain = GetComponent<AudioListener>();
        }

        private void Start()
        {
            var listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var listener in listeners)
            {
                listener.enabled = ListenerMain == listener;
            }
        }

        private void OnValidate()
        {
            transform.localPosition = offset;
        }
    }
}