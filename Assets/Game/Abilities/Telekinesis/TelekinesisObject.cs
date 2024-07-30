using UnityEngine;

namespace Game.Abilities.Telekinesis
{
    public class TelekinesisObject : MonoBehaviour
    {
        [SerializeField] private bool _paused = false;

        public bool paused
        {
            get => _paused;
            set => _paused = value;
        }
    }
}