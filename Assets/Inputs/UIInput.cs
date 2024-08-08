using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    [CreateAssetMenu(menuName = "Game/Inputs/Create UI Input", fileName = nameof(UIInput), order = 0)]
    public class UIInput : ScriptableObject, GameInput.IUIActions
    {
        private GameInput _gameInput;
        
        public event Action<bool> CancelEvent;
        
        public void OnNavigate(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CancelEvent?.Invoke(true); 
            }
            else if (context.canceled)
            {
                CancelEvent?.Invoke(false);
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
        }

        public void OnClick(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
        }
        
        private void OnEnable()
        {
            if (_gameInput == null)
            {
                _gameInput = new GameInput();
            }
            
            _gameInput.UI.SetCallbacks(this);
            _gameInput.UI.Enable();
        }

        private void OnDisable()
        {
            if (_gameInput != null)
            {
                _gameInput.UI.RemoveCallbacks(this);
                _gameInput.UI.Disable();
            }
        }
    }
}