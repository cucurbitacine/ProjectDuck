using Game.Abilities.Electricity;
using TMPro;
using UnityEngine;

namespace Game.Dev
{
    public class ElectricityChargeDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text chargeText;
        [SerializeField] private GameObject root;

        private IElectricityStorage _storage;

        private void HandleCharge(int value)
        {
            if (chargeText)
            {
                chargeText.text = $"{value}";
            }
        }

        private void Awake()
        {
            if (root == null) root = gameObject;
            _storage = root.GetComponent<IElectricityStorage>();
        }

        private void OnEnable()
        {
            if (_storage != null)
            {
                _storage.OnChargeChanged += HandleCharge;

                HandleCharge(_storage.ElectricityCharge);
            }
        }

        private void OnDisable()
        {
            if (_storage != null)
            {
                _storage.OnChargeChanged -= HandleCharge;
            }
        }
    }
}