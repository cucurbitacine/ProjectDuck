using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.SFX
{
    [CreateAssetMenu(menuName = "Game/Sound/Create Sound Profile", fileName = nameof(SoundProfile), order = 0)]
    public class SoundProfile : ScriptableObject
    {
        [SerializeField] private PlayMode playMode = PlayMode.Random;
        
        [Space]
        [SerializeField] private int index = 0;
        [SerializeField] private List<AudioClip> clips = new List<AudioClip>();

        public int Count => clips?.Count ?? 0;
        
        public AudioClip GetAudioClip()
        {
            index = EvaluateIndex(index, Count, playMode);
            
            if (index < 0 || Count <= index) return null;

            return clips[index];
        }

        private void Awake()
        {
            index = 0;
        }

        private void OnValidate()
        {
            index = Mathf.Max(0, index);
            index = Mathf.Min(index, Count);
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
    }
}