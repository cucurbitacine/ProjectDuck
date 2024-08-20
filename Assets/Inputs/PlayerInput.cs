using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    [CreateAssetMenu(menuName = "Game/Inputs/Create Player Input", fileName = nameof(PlayerInput), order = 0)]
    public class PlayerInput : ScriptableObject, GameInput.IPlayerActions
    {
        
        public event Action<Vector2> MoveEvent;
        public event Action<Vector2> LookEvent;
        public event Action<Vector2> ScreenPointEvent;
        
        public event Action<bool> PrimaryFireEvent;
        public event Action<bool> SecondaryFireEvent;
        public event Action<bool> JumpEvent;
        public event Action<bool> InteractEvent;
        
        public event Action<float> ZoomEvent;
        
        private GameInput _gameInput;
        
        public Vector2 ScreenPoint => _gameInput != null ? _gameInput.Player.ScreenPoint.ReadValue<Vector2>() : Vector2.zero;
        public Vector2 WorldPoint => CameraMain ? CameraMain.ScreenToWorldPoint(ScreenPoint) : Vector2.zero;
        
        private static Camera CameraMain => Camera.main;
        
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnPrimaryFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PrimaryFireEvent?.Invoke(true); 
            }
            else if (context.canceled)
            {
                PrimaryFireEvent?.Invoke(false);
            }
        }

        public void OnSecondaryFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SecondaryFireEvent?.Invoke(true);
            }
            else if (context.canceled)
            {
                SecondaryFireEvent?.Invoke(false);
            }
        }

        public void OnScreenPoint(InputAction.CallbackContext context)
        {
            ScreenPointEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                JumpEvent?.Invoke(true);
            }
            else if (context.canceled)
            {
                JumpEvent?.Invoke(false);
            }
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            ZoomEvent?.Invoke(context.ReadValue<Vector2>().y);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InteractEvent?.Invoke(true);
            }
            else if (context.canceled)
            {
                InteractEvent?.Invoke(false);
            }
        }

        private void OnEnable()
        {
            if (_gameInput == null)
            {
                _gameInput = new GameInput();
            }
            
            _gameInput.Player.SetCallbacks(this);
            _gameInput.Player.Enable();
        }

        private void OnDisable()
        {
            if (_gameInput != null)
            {
                _gameInput.Player.RemoveCallbacks(this);
                _gameInput.Player.Disable();
            }
        }
    }
}
