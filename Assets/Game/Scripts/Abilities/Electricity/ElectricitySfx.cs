using System;
using Game.Scripts.SFX;
using UnityEngine;

namespace Game.Scripts.Abilities.Electricity
{
    [RequireComponent(typeof(IElectricityStorage))]
    public class ElectricitySfx : MonoBehaviour
    {
        [SerializeField] private SoundFX chargedSfx;
        
        private IElectricityStorage _storage;

        private void HandleCharge(int charge)
        {
            if (_storage.ElectricityChargeMax > 0 && charge == _storage.ElectricityChargeMax)
            {
                chargedSfx.Play();
            }
            else
            {
                chargedSfx.Stop();
            }
        }
        
        private void Awake()
        {
            _storage = GetComponent<IElectricityStorage>();
        }

        private void OnEnable()
        {
            _storage.OnChargeChanged += HandleCharge;
            
            HandleCharge(_storage.ElectricityCharge);
        }

        private void OnDisable()
        {
            _storage.OnChargeChanged -= HandleCharge;
        }
    }
}