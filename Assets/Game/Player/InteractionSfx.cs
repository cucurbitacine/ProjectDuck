using Game.SFX;
using UnityEngine;

namespace Game.Player
{
    public class InteractionSfx : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField] private SoundFX quackSfx;
        
        [Header("References")]
        [SerializeField] private InteractorController interactor;
        
        private void HandleInteraction()
        {
            quackSfx.PlaySfx();
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