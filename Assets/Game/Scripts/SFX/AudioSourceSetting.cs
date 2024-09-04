using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.SFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceSetting : MonoBehaviour
    {
        private enum SpatialType
        {
            Flat,
            Surrounded, 
        }

        private enum RolloffType
        {
            Linear,
            Logarithmic, 
        }
        
        [SerializeField] private SoundGroup soundGroup = null;
        
        [Space]
        [SerializeField] private SpatialType spatialType = SpatialType.Surrounded;
        
        [Header("Surrounded Settings Only")]
        [SerializeField] [Range(0f, 1f)] private float spreadBlend = 0.8f;
        
        [Space]
        [SerializeField] private RolloffType rolloffType = RolloffType.Linear;
        [SerializeField] [Range(0f, 5f)] private float dopplerLevel = 0f;
        
        [Space]
        [SerializeField] [Min(0f)] private float minDistance = 5f;
        [SerializeField] [Min(0f)] private float maxDistance = 20f;
        
        private AudioSource _audio;
        
        private float spatialBlend => spatialType == SpatialType.Surrounded ? 1f : 0f;
        private float spread => Mathf.Lerp(0f, 180f, spreadBlend);
        
        private void SetupAudio()
        {
            if (_audio == null) _audio = GetComponent<AudioSource>();

            if (soundGroup && soundGroup.AudioMixerGroup)
            {
                _audio.outputAudioMixerGroup = soundGroup.AudioMixerGroup;
            }
            
            _audio.spatialBlend = spatialBlend;
            _audio.spread = spread;

            _audio.minDistance = minDistance;
            _audio.maxDistance = maxDistance;
            
            _audio.dopplerLevel = dopplerLevel;
            _audio.rolloffMode = rolloffType == RolloffType.Linear
                ? AudioRolloffMode.Linear
                : AudioRolloffMode.Logarithmic;
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
            if (_audio && spatialType == SpatialType.Surrounded)
            {
                Gizmos.color = new Color(0.2f, 0.2f, 0.6f);
                
                Tools.DrawCircle2D(_audio.transform.position, minDistance);
                Tools.DrawCircle2D(_audio.transform.position, maxDistance);
            }
        }
    }
}