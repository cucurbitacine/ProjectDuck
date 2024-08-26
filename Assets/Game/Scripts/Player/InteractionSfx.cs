using Game.Scripts.SFX;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class InteractionSfx : MonoBehaviour
    {
        [SerializeField] private InteractorController interactor;
        
        [Header("SFX")]
        [SerializeField] private SoundFX sfx;
        
        private void HandleInteraction()
        {
            sfx.Play();
        }
        
        private void OnEnable()
        {
            interactor.OnInteracted += HandleInteraction;
        }
        
        private void OnDisable()
        {
            interactor.OnInteracted -= HandleInteraction;
        }
    }
}