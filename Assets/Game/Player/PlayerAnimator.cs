using Game.Movements;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimator : MonoBehaviour, IMovement2DHandle
    {
        [SerializeField] private Movement2D movement;
        [SerializeField] private Animator animator;
        
        private static readonly int Moving = Animator.StringToHash("Moving");

        public void SetMovement2D(Movement2D movement2D)
        {
            movement = movement2D;
        }
        
        private void LateUpdate()
        {
            if (animator && movement)
            {
                animator.SetBool(Moving, movement.isMoving);
            }
        }
    }
}