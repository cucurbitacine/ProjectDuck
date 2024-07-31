using Game.Movements;
using UnityEngine;

namespace Game.Utils
{
    [DisallowMultipleComponent]
    public class SpriteFlip : MonoBehaviour, IMovement2DHandle
    {
        public bool inverse = false;
        
        [Space]
        public Movement2D movement;
        
        public void Flip(float x)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(x) * (inverse ? -1 : 1);
            transform.localScale = scale;
        }

        public void SetMovement2D(Movement2D movement2D)
        {
            movement = movement2D;
        }
        
        private void LateUpdate()
        {
            if (movement && movement.isMoving)
            {
                Flip(movement.move.x);
            }
        }
    }
}