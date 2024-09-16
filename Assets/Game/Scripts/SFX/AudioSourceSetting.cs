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

        public AudioSettings GetAudioSettings()
        {
            if (useCustomSettings)
            {
                return customSettings;
            }
            
            return soundGroup ? soundGroup.GetSettings() : customSettings;
        }

        public float GetVolume()
        {
#if !UNITY_WEBGL
            return 1f;
#endif
            
            var settings = GetAudioSettings();

            if (settings.spatialType == SpatialType.Flat) return 1f;

            if (settings.spreadBlend >= 1f) return 1f;
            
            if (!MainAudioListener.Main) return 1f;

            var distance = Vector3.Distance(MainAudioListener.Main.transform.position,
                _audio ? _audio.transform.position : transform.position);

            if (distance < settings.minDistance) return 1f;

            if (distance < settings.maxDistance) return 1f - (distance - settings.minDistance) / (settings.maxDistance - settings.minDistance);

            return 0f;
        }

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
                var settings = GetAudioSettings();
                
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