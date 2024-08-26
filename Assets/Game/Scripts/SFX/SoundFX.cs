using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Game.Scripts.SFX
{
    public sealed class SoundFX : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [Space]
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        
        [Header("Settings")]
        [SerializeField] private bool playOneShot = true;
        [SerializeField] [Range(0f, 1f)] private float volume = 1f;
        
        [Header("Background")]
        [SerializeField] private bool loop;
        [Space]
        [SerializeField] private bool playOnEnable;
        [SerializeField] private bool stopOnDisable;
        [Space]
        [SerializeField] [Min(0f)] private float easeInOut = 0.5f;
        
        [Header("Sound")]
        [SerializeField] private SoundProfile soundProfile;
        [SerializeField] private PlayMode playMode = PlayMode.Random;
        
        [Space]
        [SerializeField] private AudioClip defaultAudioClip;

        public bool IsPlaying => audioSource && audioSource.isPlaying;
        public bool OnPause { get; private set; }
        
        private int _index = 0;
        private Coroutine _easing;
        
        [ContextMenu(nameof(PlaySfx))]
        public void PlaySfx()
        {
            if (playOneShot)
            {
                audioSource.PlayOneShot(GetAudioClip(), volume);
            }
            else
            {
                if (OnPause)
                {
                    OnPause = false;
                    audioSource.UnPause();
                }
                else
                {
                    PlayOrStop(true);
                }
            }
        }

        [ContextMenu(nameof(PauseSfx))]
        public void PauseSfx()
        {
            if (playOneShot) return;

            OnPause = true;
            audioSource.Pause();
        }

        [ContextMenu(nameof(StopSfx))]
        public void StopSfx()
        {
            if (OnPause)
            {
                OnPause = false;
                audioSource.Stop();
            }
            else
            {
                PlayOrStop(false);
            }
        }

        public void SetSoundProfile(SoundProfile profile)
        {
            soundProfile = profile;
        }

        private void PlayOrStop(bool play)
        {
            if (gameObject.activeInHierarchy)
            {
                if (_easing != null) StopCoroutine(_easing);
                _easing = StartCoroutine(EasePlayOrStop(play));
            }
            else
            {
                if (play)
                {
                    audioSource.volume = volume;
                    audioSource.clip = GetAudioClip();
                    audioSource.Play();
                }
                else
                {
                    audioSource.Stop();
                }
            }
        }
        
        private IEnumerator EasePlayOrStop(bool play)
        {
            var origin = audioSource.isPlaying ? audioSource.volume : (play ? 0f : volume);
            var target = play ? volume : 0f;
            
            var speed = volume / easeInOut;
            var distance = Mathf.Abs(target - origin);
            var duration = distance / speed;
            
            if (play)
            {
                audioSource.clip = GetAudioClip();
                audioSource.Play();
            }
            
            var timer = 0f;
            while (timer < duration)
            {
                var t = Mathf.SmoothStep(0, 1f, timer / duration);
                audioSource.volume = Mathf.Lerp(origin, target, t);
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            audioSource.volume = target;

            if (!play)
            {
                audioSource.Stop();
            }
        }
        
        private AudioClip GetAudioClip()
        {
            if (soundProfile)
            {
                _index = EvaluateIndex(_index, soundProfile.Count, playMode);
                return soundProfile.GetAudioClip(_index);
            }

            return defaultAudioClip;
        }

        private static int EvaluateIndex(int index, int count, PlayMode playMode)
        {
            if (count <= 0) return -1;

            if (playMode == PlayMode.Sequence)
            {
                return (index + 1) % count;
            }

            if (playMode == PlayMode.Random)
            {
                return Random.Range(0, count);
            }

            return index;
        }
        
        private void Awake()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = GetComponentInParent<AudioSource>();

            if (audioMixerGroup)
            {
                audioSource.outputAudioMixerGroup = audioMixerGroup;
            }
            
            if (playOneShot)
            {
                loop = false;
                playOnEnable = false;
                stopOnDisable = false;
            }
            else 
            {
                audioSource.loop = loop;
                audioSource.volume = volume;
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

            if (Application.isPlaying && audioSource)
            {
                if (playOneShot)
                {
                    
                }
                else
                {
                    audioSource.loop = loop;
                    audioSource.volume = volume;
                }
            }
        }

        private void OnEnable()
        {
            if (playOnEnable)
            {
                PlaySfx();
            }
        }

        private void OnDisable()
        {
            if (_easing != null) StopCoroutine(_easing);
            
            if (stopOnDisable)
            {
                OnPause = false;
                audioSource.Stop();
            }
        }
    }
}