using Game.Scripts.SFX;
using UnityEngine;

namespace Game.Scripts.Interactions.Impl
{
    [RequireComponent(typeof(DoorController))]
    public class DoorSfx : MonoBehaviour
    {
        [SerializeField] private SoundFX startMovingSfx;
        [SerializeField] private SoundFX stopMovingSfx;
        
        [Space]
        [SerializeField] private SoundFX movingSfx;
        
        private DoorController _door;

        private void HandleDoor(bool move)
        {
            if (move)
            {
                startMovingSfx?.Play();
                
                movingSfx?.Play(true);
            }
            else
            {
                stopMovingSfx?.Play();
                
                movingSfx?.Play(false);
            }
        }
        
        private void Awake()
        {
            _door = GetComponent<DoorController>();
        }

        private void OnEnable()
        {
            _door.OnMoved += HandleDoor;
        }
        
        private void OnDisable()
        {
            _door.OnMoved -= HandleDoor;
        }
    }
}