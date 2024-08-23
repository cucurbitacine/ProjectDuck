using System.Collections.Generic;
using UnityEngine;

namespace Game.SFX
{
    [CreateAssetMenu(menuName = "Game/Sound/Create Sound Profile", fileName = nameof(SoundProfile), order = 0)]
    public class SoundProfile : ScriptableObject
    {
        [SerializeField] private List<AudioClip> clips = new List<AudioClip>();

        public int Count => clips?.Count ?? 0;
        
        public AudioClip GetAudioClip(int index)
        {
            if (index < 0 || Count <= index) return null;

            return clips[index];
        }
    }
}