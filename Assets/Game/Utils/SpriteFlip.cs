using System;
using Game.Movements;
using Game.Player;
using UnityEngine;

namespace Game.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFlip : MonoBehaviour, IPlayerHandle
    {
        public bool inverse = false;
        
        [Space]
        public PlayerController player;

        private SpriteRenderer _sprite;

        public void Flip(bool value)
        {
            if (_sprite.flipX == value) return;
            
            _sprite.flipX = value;
        }
        
        public void Flip(float x)
        {
            if (x > 0f)
            {
                Flip(inverse);
            }
            else if (x < 0f)
            {
                Flip(!inverse);
            }
        }

        public void SetPlayer(PlayerController newPlayer)
        {
            player = newPlayer;
        }

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
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