using Game.DamageSystem;
using UnityEngine;

namespace Game.Combat
{
    public class DamageModifier : MonoBehaviour, IDamageModifier
    {
        [Min(0f)]
        [SerializeField] private float multiply = 1f;
        [SerializeField] private int addition = 0;
        [SerializeField] private bool canBeZero = true;

        public void ModifyDamage(Damage damage)
        {
            damage.amount = (int)(damage.amount * multiply) + addition;

            damage.amount = Mathf.Max(canBeZero ? 0 : 1, damage.amount);
        }
    }
}