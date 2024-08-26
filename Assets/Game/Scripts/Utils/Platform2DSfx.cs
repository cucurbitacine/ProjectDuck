using Game.Scripts.SFX;
using UnityEngine;

namespace Game.Scripts.Utils
{
    [RequireComponent(typeof(PlatformMovement2D))]
    public class Platform2DSfx : MonoBehaviour
    {
        [SerializeField] private SoundFX startMoveSfx;
        [SerializeField] private SoundFX stopMoveSfx;
        [Space]
        [SerializeField] private SoundFX movingSfx;

        private PlatformMovement2D _platform;
        
        private void HandlePlatform(bool move)
        {
            if (move)
            {
                startMoveSfx?.Play();
            }
            else
            {
                stopMoveSfx?.Play();
            }

            movingSfx?.Play(move);
        }
        
        private void Awake()
        {
            _platform = GetComponent<PlatformMovement2D>();
        }

        private void OnEnable()
        {
            _platform.OnMoved += HandlePlatform;

            HandlePlatform(_platform.IsMoving);
        }
        
        private void OnDisable()
        {
            _platform.OnMoved -= HandlePlatform;
        }
    }
}