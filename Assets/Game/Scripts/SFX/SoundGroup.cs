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

        public AudioMixerGroup AudioMixerGroup => AudioMixer ? AudioMixer.outputAudioMixerGroup : null;
    }
}