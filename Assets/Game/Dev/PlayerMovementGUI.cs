using Game.Scripts.Movements;
using UnityEngine;

namespace Game.Dev
{
    public class PlayerMovementGUI : MonoBehaviour
    {
        public bool customRect = false;
        public Rect rect = new Rect(0, 0, 100, 100);
        public Movement2D movement;

        public static Camera CameraMain => Camera.main;

        public static Vector2 ScreenSize => new Vector2(Screen.width, Screen.height);

        private Rect GetRect()
        {
            if (customRect) return rect;

            var size = rect.size;
            size.x = Mathf.Clamp(size.x, 0, ScreenSize.x);
            size.y = Mathf.Clamp(size.y, 0, ScreenSize.y);
            
            var position = ScreenSize - size;
            position.x = Mathf.Max(position.x, 0);
            position.y = Mathf.Max(position.y, 0);
            
            rect.position = position;
            rect.size = size;
            
            return rect;
        }
        
        private void OnGUI()
        {
            if (!movement) return;

            var speed = movement.velocity.magnitude;
            
            GUILayout.BeginArea(GetRect());
            
            GUILayout.Toggle(movement.isMoving, "Moving");
            GUILayout.Toggle(movement.isJumping, "Jumping");
            GUILayout.Toggle(movement.isFalling, "Falling");
            GUILayout.Toggle(movement.isGrounded, "On Ground");
            GUILayout.Toggle(movement.onSurface, "On Surface");
            GUILayout.Toggle(movement.onSlope, "On Slope");
            GUILayout.Toggle(movement.onInertialGround, "On Platform");
            GUILayout.Box($"{speed:F1} u/s");
            
            GUILayout.EndArea();

            if (GUILayout.Button("Jump"))
            {
                movement.Jump();
            }
        }
    }
}