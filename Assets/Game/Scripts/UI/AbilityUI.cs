using System;
using Game.Scripts.Abilities;
using UnityEngine;

namespace Game.Scripts.UI.Abilities
{
    public class AbilityUI : MonoBehaviour, IAbilityUI
    {
        [SerializeField] private AbilityBase ability;

        public event Action<AbilityBase> OnAbilityChanged;

        public AbilityBase GetAbility()
        {
            return ability;
        }
        
        public void SetAbility(AbilityBase newAbility)
        {
            ability = newAbility;
            
            OnAbilityChanged?.Invoke(ability);
        }

        public void ResetAbility()
        {
            ability = null;
            
            Destroy(gameObject);
        }
    }
}