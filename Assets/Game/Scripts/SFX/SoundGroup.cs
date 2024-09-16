using UnityEngine;
using UnityEngine.Audio;

namespace Game.Scripts.SFX
{
    [CreateAssetMenu(menuName = "Game/Sound/Create Sound Group", fileName = nameof(SoundGroup), order = 0)]
    public class SoundGroup : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; private set; } = string.Empty;

        [field: Space]
        [field: SerializeField] public AudioMixer AudioMixer { get; private set; }

        [Space]
        [SerializeField] private AudioSettings audioSettings = new AudioSettings();
        
        public AudioMixerGroup AudioMixerGroup => AudioMixer ? AudioMixer.outputAudioMixerGroup : null;

        public AudioSettings GetSettings()
        {
            var settings = new AudioSettings();

            settings.Copy(audioSettings);
            
            return settings;
        }

        public void SetupAudioSource(AudioSource audioSource)
        {
            audioSource.spatialBlend = audioSettings.spatialBlend;
            audioSource.spread = audioSettings.spread;

            audioSource.minDistance = audioSettings.minDistance;
            audioSource.maxDistance = audioSettings.maxDistance;
            
            audioSource.dopplerLevel = audioSettings.dopplerLevel;
            audioSource.rolloffMode = audioSettings.rolloffMode;
                
            if (AudioMixerGroup)
            {
                audioSource.outputAudioMixerGroup = AudioMixerGroup;
            }
            
#if UNITY_WEBGL
            audioSource.spatialBlend = 0f;
            audioSource.spread = 180f;
#endif
        }
    }
}