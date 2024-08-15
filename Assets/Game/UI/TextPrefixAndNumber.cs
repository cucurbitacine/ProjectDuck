using Game.Core;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class TextPrefixAndNumber : MonoBehaviour
    {
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private NumberType numberType = NumberType.PlayerId;
        
        public enum NumberType
        {
            PlayerId,
            AttemptNumber,
        }
        
        [Space]
        [SerializeField] private TMP_Text text;

        private async void UpdateText()
        {
            if (!text) return;

            var playerData = await GameManager.Instance.GetPlayerDataAsync();

            var number = numberType == NumberType.PlayerId ? playerData.playerId : playerData.attemptNumber;
            
            text.text = $"{prefix}{number}";
        }
        
        private void Start()
        {
            UpdateText();
        }

        private void OnValidate()
        {
            text.text = $"{prefix}{(numberType == NumberType.PlayerId ? "000" : "00")}";
        }
    }
}
