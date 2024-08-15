using Game.Movements;
using Game.Player;
using UnityEngine;

namespace Game.Utils
{
    [DisallowMultipleComponent]
    public class SpriteFlip : MonoBehaviour, IPlayerHandle
    {
        public bool inverse = false;
        
        [Space]
        public PlayerController player;
        
        public void Flip(float x)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(x) * (inverse ? -1 : 1);
            transform.localScale = scale;
        }

        public void SetPlayer(PlayerController newPlayer)
        {
            player = newPlayer;
        }
        
        private void LateUpdate()
        {
            if (player && player.GetMovement2D().isMoving)
            {
                Flip(player.GetMovement2D().move.x);
            }
        }
    }
}