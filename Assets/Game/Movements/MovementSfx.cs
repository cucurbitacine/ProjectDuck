using Game.SFX;
using UnityEngine;

namespace Game.Movements
{
    public class MovementSfx : MonoBehaviour
    {
        [SerializeField] [Min(0.01f)] private float rate = 1f;

        [Header("SFX")]
        [SerializeField] private SoundFX stepSfx;
        
        [Header("References")]
        [SerializeField] private Movement2D movement2D;
        [SerializeField] private Ground2D ground2D;
        
        private float _timer;
        private float _lastTime;
        
        private float timeout => rate > 0f ? 1f / rate : 1f;

        private void Step(float time)
        {
            stepSfx.PlaySfx();

            _lastTime = time;
        }

        private void HandleMovement(float deltaTime)
        {
            if (movement2D.isMoving && movement2D.isGrounded)
            {
                if (_timer < 0f)
                {
                    _timer = timeout;

                    Step(Time.time);
                }
                else
                {
                    _timer -= deltaTime;
                }
            }
        }
        
        private void HandleGround(bool grounded)
        {
            var time = Time.time;
            
            if (grounded && time - _lastTime > timeout)
            {
                Step(time);
            }
        }
        
        private void OnEnable()
        {
            ground2D.OnGrounded += HandleGround;
        }
        
        private void OnDisable()
        {
            ground2D.OnGrounded -= HandleGround;
        }
        
        private void Update()
        {
            HandleMovement(Time.deltaTime);
        }
    }
}