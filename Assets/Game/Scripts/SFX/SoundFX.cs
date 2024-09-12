using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.SFX
{
    public sealed class SoundFX : MonoBehaviour
    {
        /*
         * Two Modes: SFX and Ambient
         *
         * SFX
         * Use only PlayOneShot
         * Stop do not work
         *
         * Ambient
         * Can be looped
         * Ease In Out
         * 
         */
        
        public enum SoundType
        {
            PlayOneShot,
            Continuous, 
        }
        
        private enum SoundState
        {
            Stopped,
            Playing,
        }
        
        [SerializeField] private AudioSource audioSource;
        
        [Header("Sound")]
        [SerializeField] private SoundProfile soundProfile;
        [Space]
        [SerializeField] private SoundType soundType = SoundType.PlayOneShot;
        [SerializeField] private AudioClip defaultAudioClip;
        
        [Header("Settings")]
        [SerializeField] [Range(0f, 1f)] private float volumeScale = 1f;
        [SerializeField] private bool playOnEnable;
        
        [Header("Continuous Settings Only")]
        [SerializeField] private bool loop;
        [SerializeField] [Min(0f)] private float easeInOut = 0.5f;
        
        private Coroutine _playingContinuous;
        
        [field: SerializeField, Space] private SoundState State { get; set; }
        
        public bool IsPlaying
        {
            get => State == SoundState.Playing;
            private set => State = value ? SoundState.Playing : SoundState.Stopped;
        }
        
        public Coroutine Play(float inOut)
        {
            if (soundType == SoundType.Continuous)
            {
                return PlayContinuous(SoundState.Playing, easeInOut);
            }
            
            if (soundType == SoundType.PlayOneShot)
            {
                PlayOneShot();
            }
            
            return null;
        }

        public Coroutine Stop(float inOut)
        {
            if (soundType == SoundType.Continuous)
            {
                return PlayContinuous(SoundState.Stopped, inOut);
            }

            return null;
        }
        
        [ContextMenu(nameof(Play))]
        public Coroutine Play()
        {
            return Play(easeInOut);
        }

        [ContextMenu(nameof(Stop))]
        public Coroutine Stop()
        {
            return Stop(easeInOut);
        }
        
        public Coroutine Play(bool play)
        {
            if (play && !IsPlaying)
            {
                return Play();
            }
            
            if (!play && IsPlaying)
            {
                return Stop();
            }

            return null;
        }
        
        private Coroutine PlayContinuous(SoundState state, float inOut)
        {
            State = state;
            
            if (_playingContinuous != null) StopCoroutine(_playingContinuous);
            
            if (inOut > 0f)
            {
                _playingContinuous = StartCoroutine(PlayingContinuous(state, inOut));

                return _playingContinuous;
            }
            
            if (state == SoundState.Playing)
            {
                PlayAudioSource();
            }
                
            if (state == SoundState.Stopped)
            {
                StopAudioSource();
            }

            return null;
        }
        
        private IEnumerator PlayingContinuous(SoundState soundState, float easeDuration)
        {
            var volume = GetVolume();
            
            var origin = audioSource.isPlaying ? audioSource.volume : (soundState == SoundState.Playing ? 0f : volume);
            var target = soundState == SoundState.Playing ? volume : 0f;
            
            if (soundState == SoundState.Playing)
            {
                PlayAudioSource();
            }
            
            var speed = volume / easeDuration;
            var distance = Mathf.Abs(target - origin);
            var duration = distance / speed;
            
            var timer = 0f;
            while (timer < duration)
            {
                var t = Mathf.SmoothStep(0, 1f, timer / duration);
                audioSource.volume = Mathf.Lerp(origin, target, t);
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            audioSource.volume = target;

            if (soundState == SoundState.Stopped)
            {
                StopAudioSource();
            }
            
            audioSource.volume = volume;
        }

        private void PlayOneShot()
        {
            var clip = GetAudioClip();

            if (!clip)
            {
                Debug.LogWarning($"\"{name} ({nameof(SoundFX)})\" Clip is Null");
                return;
            }
            
            audioSource.PlayOneShot(clip, GetVolume());
        }
        
        private void PlayAudioSource()
        {
            var clip = GetAudioClip();

            if (!clip)
            {
                Debug.LogWarning($"\"{name} ({nameof(SoundFX)})\" Clip is Null");
                return;
            }

            audioSource.clip = clip;
            audioSource.Play();
        }

        private void StopAudioSource()
        {
            audioSource.Stop();
        }

        private float GetVolume()
        {
            if (soundProfile)
            {
                return soundProfile.Volume * volumeScale;
            }
            
            return volumeScale;
        }
        
        private AudioClip GetAudioClip()
        {
            if (soundProfile)
            {
                return soundProfile.GetAudioClip();
            }

            return defaultAudioClip;
        }
        
        private void Awake()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = GetComponentInParent<AudioSource>();

            audioSource.playOnAwake = false;
            
            if (soundType == SoundType.PlayOneShot)
            {
                loop = false;
            }
            
            if (soundType == SoundType.Continuous)
            {
                audioSource.loop = loop;
                audioSource.volume = GetVolume();
            }

            if (soundProfile && soundProfile.Count == 0)
            {
                Debug.LogWarning($"\"{name} ({nameof(SoundFX)})\" without any audio clips");
            }
        }

        private void OnValidate()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = GetComponentInParent<AudioSource>();

            if (audioSource)
            {
                audioSource.playOnAwake = false;
                
                if (Application.isPlaying)
                {
                    if (soundType == SoundType.Continuous)
                    {
                        audioSource.loop = loop;
                        audioSource.volume = GetVolume();
                    }
                }
            }
        }

        private void OnEnable()
        {
            Register(this);
            
            if (playOnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            State = SoundState.Stopped;

            Unregister(this);
        }

        private static readonly Dictionary<AudioSource, HashSet<SoundFX>> Dict = new Dictionary<AudioSource, HashSet<SoundFX>>();

        private static HashSet<SoundFX> GetSet(AudioSource source)
        {
            if (Dict.TryGetValue(source, out var set)) return set;
            
            set = new HashSet<SoundFX>();
            Dict.Add(source, set);

            return set;
        }

        private static bool Register(SoundFX sfx)
        {
            var set = GetSet(sfx.audioSource);

            if (sfx.soundType == SoundType.Continuous && set.Count > 0)
            {
                Debug.LogWarning($"\"{sfx.name}\" with [{nameof(SoundType)}.{sfx.soundType.ToString()}] cannot be attached to \"{sfx.audioSource.name}\" {nameof(AudioSource)}");
            }

            return set.Add(sfx);
        }

        private static bool Unregister(SoundFX sfx)
        {
            var set = GetSet(sfx.audioSource);

            return set.Remove(sfx);
        }
    }
}