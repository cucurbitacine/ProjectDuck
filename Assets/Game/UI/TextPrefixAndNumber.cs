using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.UI
{
    public class TextPrefixAndNumber : MonoBehaviour
    {
        [SerializeField] private string prefix = string.Empty;
        
        [Space]
        [SerializeField] private int minNumber = 100;
        [SerializeField] private int maxNumber = 999;
        
        [Space]
        [SerializeField] private TMP_Text text;

        private void UpdateText()
        {
            if (!text) return;

            text.text = $"{prefix}{Random.Range(minNumber, maxNumber + 1)}";
        }
        
        private void Start()
        {
            UpdateText();
        }

        private void OnValidate()
        {
            UpdateText();
        }
    }
}
