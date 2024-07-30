using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    [CreateAssetMenu(menuName = "Game/Inputs/Create Player Actions Profile", fileName = nameof(PlayerActionsProfile), order = 0)]
    public class PlayerActionsProfile : ScriptableObject, GameInput.IPlayerActions
    {
        public Vector2 ScreenPoint { get; private set; }
        
        public event Action<Vector2> MoveEvent;
        public event Action<Vector2> LookEvent;
        public event Action<Vector2> ScreenPointEvent;
        
        public event Action<bool> PrimaryFireEvent;
        public event Action<bool> SecondaryFireEvent;
        public event Action<bool> JumpEvent;
        public event Action<bool> InteractEvent;
        
        public event Action<float> ZoomEvent;
        
        private GameInput gameInput;
        
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

        public void OnPoint(InputAction.CallbackContext context)
        {
            ScreenPoint = context.ReadValue<Vector2>();
            
            ScreenPointEvent?.Invoke(ScreenPoint);
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
            if (gameInput == null)
            {
                gameInput = new GameInput();
            }
            
            gameInput.Player.SetCallbacks(this);
            gameInput.Player.Enable();
        }

        private void OnDisable()
        {
            if (gameInput != null)
            {
                gameInput.Player.RemoveCallbacks(this);
                gameInput.Player.Disable();
            }
        }
    }
}
