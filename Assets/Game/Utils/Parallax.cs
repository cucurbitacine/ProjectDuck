using UnityEngine;

namespace Game.Utils
{
    public class Parallax : MonoBehaviour
    {

        public bool pausedX = false;
        public bool pausedY = true;
        
        [Space]
        [Range(0f, 1f)] public float xParallax = 0.5f;
        [Range(0f, 1f)] public float yParallax = 0.5f;

        private Vector2 spriteSize;
        private Vector2 startPosition;

        public static Camera CameraMain => Camera.main;
        public static Vector2 CameraPosition => CameraMain.transform.position;
        
        private void UpdateX()
        {
            if (pausedX) return;
            
            var distance = CameraPosition.x * xParallax;

            var position = transform.position;
            position.x = startPosition.x + distance;
            transform.position = position;

            var temp = CameraPosition.x * (1f - xParallax);

            if (temp > startPosition.x + spriteSize.x * 0.5f)
            {
                startPosition.x += spriteSize.x;
            }
            else if (temp < startPosition.x - spriteSize.x * 0.5f)
            {
                startPosition.x -= spriteSize.x;
            }
        }
        
        private void UpdateY()
        {
            if (pausedY) return;
            
            var distance = CameraPosition.y * yParallax;

            var position = transform.position;
            position.y = startPosition.y + distance;
            transform.position = position;

            var temp = CameraPosition.y * (1f - yParallax);

            if (temp > startPosition.y + spriteSize.y * 0.5f)
            {
                startPosition.y += spriteSize.y;
            }
            else if (temp < startPosition.y - spriteSize.y * 0.5f)
            {
                startPosition.y -= spriteSize.y;
            }
        }
        
        private void Start()
        {
            startPosition = transform.position;
            spriteSize = GetComponent<SpriteRenderer>()?.bounds.size ?? Vector2.zero;
        }

        private void LateUpdate()
        {
            UpdateX();
            
            UpdateY();
        }
    }
}