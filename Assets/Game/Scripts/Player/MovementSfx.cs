using Game.Scripts.Movements;
using Game.Scripts.SFX;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class MovementSfx : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float minTimeBetweenStep = 0.2f;
        [SerializeField] [Min(0f)] private float minTimeBetweenGround = 0.2f;
        
        [Header("References")]
        [SerializeField] private PlayerController player;
        
        [Header("SFX")]
        [SerializeField] private SoundFX groundSfx;
        [SerializeField] private SoundFX stepSfx;
        [SerializeField] private SoundFX jumpSfx;
        
        private ModelLoader _modelLoader;
        private Movement2D _movement;
        private StepHandler _stepHandler;

        private float _lastStepTime = float.MinValue;
        private float _lastGroundTime = float.MinValue;
        
        private void HandleStep()
        {
            var time = Time.time;

            if (time - _lastStepTime > minTimeBetweenStep && time - _lastGroundTime > minTimeBetweenStep)
            {
                _lastStepTime = time;
                
                stepSfx.Play();
            }
        }

        private void HandleGround(bool grounded)
        {
            if (grounded)
            {
                var time = Time.time;
            
                if (time - _lastGroundTime > minTimeBetweenGround)
                {
                    _lastGroundTime = time;
                
                    groundSfx.Play();
                }
            }
        }

        private void HandleJump()
        {
            jumpSfx.Play();
        }
        
        private void HandleModel(GameObject model)
        {
            if (_stepHandler)
            {
                _stepHandler.OnStep -= HandleStep;
            }

            _stepHandler = model.GetComponent<StepHandler>();
            
            if (_stepHandler)
            {
                _stepHandler.OnStep += HandleStep;
            }
        }
        
        private void Start()
        {
            _modelLoader = player.ModelLoader;
            _modelLoader.OnModelLoaded += HandleModel;
            HandleModel(_modelLoader.GetModel());

            _movement = player.GetMovement2D();
            _movement.OnJumped += HandleJump;
            _movement.Ground2D.OnGrounded += HandleGround;
        }
        
        private void OnDestroy()
        {
            if (_modelLoader)
            {
                _modelLoader.OnModelLoaded -= HandleModel;
            }

            if (_movement)
            {
                _movement.OnJumped -= HandleJump;
                
                if (_movement.Ground2D)
                {
                    _movement.Ground2D.OnGrounded -= HandleGround;
                }
            }
        }
    }
}