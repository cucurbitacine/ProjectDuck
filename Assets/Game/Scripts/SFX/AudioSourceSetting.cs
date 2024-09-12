using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.SFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceSetting : MonoBehaviour
    {
        [SerializeField] private SoundGroup soundGroup = null;

        [Header("Custom Settings")]
        [SerializeField] private bool useCustomSettings = false;
        [SerializeField] private AudioSettings customSettings = new AudioSettings();
        
        private AudioSource _audio;
        
        private void SetupAudio()
        {
            if (_audio == null) _audio = GetComponent<AudioSource>();

            if (soundGroup)
            {
                soundGroup.SetupAudioSource(_audio);
            }
        }
        
        private void Awake()
        {
            SetupAudio();
        }

        private void OnValidate()
        {
            SetupAudio();
        }

        private void OnDrawGizmosSelected()
        {
            if (_audio)
            {
                var settings = !useCustomSettings && soundGroup ? soundGroup.GetSettings() : customSettings;
                
                if (settings.spatialType == SpatialType.Surrounded)
                {
                    Gizmos.color = new Color(0.2f, 0.2f, 0.6f);

                    Tools.DrawCircle2D(_audio.transform.position, settings.minDistance);
                    Tools.DrawCircle2D(_audio.transform.position, settings.maxDistance);
                }
            }
        }
    }
}