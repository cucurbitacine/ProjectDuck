using System;
using Game.Scripts.Abilities;
using TMPro;
using UnityEngine;

namespace Game.Dev
{
    public class Dev_PickupAbilityDisplay : MonoBehaviour
    {
        [SerializeField] private bool clickToUpdate;
        [SerializeField] private PickupAbility pickupAbility;
        [SerializeField] private TMP_Text abilityText;

        private void Display()
        {
            if (pickupAbility == null) pickupAbility = GetComponentInChildren<PickupAbility>();
            if (abilityText == null) abilityText = GetComponentInChildren<TMP_Text>();
            
            if (pickupAbility && abilityText)
            {
                var ability = pickupAbility.GetAbilityPrefab();

                if (ability)
                {
                    //var text = $"{ability.name} ({ability.GetType().Name})";
                    var text = $"{ability.GetType().Name}";

                    text = text.Replace("Ability", "");
                    
                    abilityText.text = text;
                }
            }
        }
        
        private void OnEnable()
        {
            Display();
        }

        private void OnValidate()
        {
            Display();
        }
    }
}