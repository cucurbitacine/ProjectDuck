using Game.Scripts.SFX;
using UnityEngine;

namespace Game.Scripts.Interactions.Impl
{
    [RequireComponent(typeof(ButtonController))]
    public class ButtonSfx : MonoBehaviour
    {
        [SerializeField] private SoundFX clickSfx;
        
        private ButtonController _button;

        private void HandleButton(GameObject actor)
        {
            clickSfx.Play();
        }

        private void Awake()
        {
            _button = GetComponent<ButtonController>();
        }

        private void OnEnable()
        {
            _button.OnInteracted += HandleButton;
        }
        
        private void OnDisable()
        {
            _button.OnInteracted -= HandleButton;
        }
    }
}