using CucuTools.DamageSystem;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimator : MonoBehaviour, IPlayerHandler
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private Animator animator;
        
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int Grounded = Animator.StringToHash("Grounded");
        
        private InteractorController _interactor;
        
        public void SetPlayer(PlayerController newPlayer)
        {
            if (_interactor)
            {
                _interactor.OnInteracted -= HandleInteraction;
            }

            if (player)
            {
                player.Health.OnDied -= HandleDeath;
                player.Health.OnDamageReceived -= HandleDamage;
            }
            
            player = newPlayer;

            if (player)
            {
                if (player.TryGetComponent(out _interactor))
                {
                    _interactor.OnInteracted += HandleInteraction;
                }
                
                player.Health.OnDied += HandleDeath;
                player.Health.OnDamageReceived += HandleDamage;
            }
            else
            {
                _interactor = null;
            }
        }

        private void HandleDeath()
        {
            if (animator)
            {
                animator.Play("Dead");
            }
        }

        private void HandleDamage(DamageEvent damageEvent)
        {
            if (animator && player)
            {
                if (player.Health.IsDead) return;
                if (damageEvent.damage.amount <= 0) return;
                
                if (animator)
                {
                    animator.Play("Hit");
                }
            }
        }
        
        private void HandleInteraction()
        {
            if (animator)
            {
                animator.Play("Quack");
            }
        }
        
        private void OnDisable()
        {
            SetPlayer(null);
        }

        private void LateUpdate()
        {
            if (animator && player)
            {
                animator.SetBool(Moving, player.GetMovement2D().isMoving);
                animator.SetBool(Grounded, player.GetMovement2D().isGrounded);
            }
        }
    }
}