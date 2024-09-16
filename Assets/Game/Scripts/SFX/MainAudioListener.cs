using UnityEngine;

namespace Game.Scripts.SFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioListener))]
    public class MainAudioListener : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = Vector3.zero;
        
        public static AudioListener Main { get; private set; }
        
        private void Awake()
        {
            Main = GetComponent<AudioListener>();
        }

        private void Start()
        {
            var listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var listener in listeners)
            {
                listener.enabled = Main == listener;
            }
        }

        private void OnValidate()
        {
            transform.localPosition = offset;
        }
    }
}