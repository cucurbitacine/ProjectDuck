using Game.Scripts.SFX;
using UnityEngine;

namespace Game.Scripts.Interactions.Impl
{
    [RequireComponent(typeof(PlateController))]
    public class PlateSfx : MonoBehaviour
    {
        [SerializeField] private SoundFX activateSfx;
        
        private PlateController _plate;

        private void HandlePlate(bool value)
        {
            activateSfx.Play();
        }

        private void Awake()
        {
            _plate = GetComponent<PlateController>();
        }

        private void OnEnable()
        {
            _plate.OnValueChanged += HandlePlate;
        }
        
        private void OnDisable()
        {
            _plate.OnValueChanged -= HandlePlate;
        }
    }
}