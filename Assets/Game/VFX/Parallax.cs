using UnityEngine;

namespace Game.VFX
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField] private bool pausedX = false;
        [SerializeField] private bool pausedY = true;
        
        [Space]
        [SerializeField] [Range(0f, 1f)] private  float xParallax = 0.5f;
        [SerializeField] [Range(0f, 1f)] private  float yParallax = 0.5f;

        private Vector2 _spriteSize;
        private Vector2 _startPosition;

        public static Camera CameraMain => Camera.main;
        public static Vector2 CameraPosition => CameraMain.transform.position;
        
        private void UpdateX()
        {
            if (pausedX) return;
            
            var distance = CameraPosition.x * xParallax;

            var position = transform.position;
            position.x = _startPosition.x + distance;
            transform.position = position;

            var temp = CameraPosition.x * (1f - xParallax);

            if (temp > _startPosition.x + _spriteSize.x * 0.5f)
            {
                _startPosition.x += _spriteSize.x;
            }
            else if (temp < _startPosition.x - _spriteSize.x * 0.5f)
            {
                _startPosition.x -= _spriteSize.x;
            }
        }
        
        private void UpdateY()
        {
            if (pausedY) return;
            
            var distance = CameraPosition.y * yParallax;

            var position = transform.position;
            position.y = _startPosition.y + distance;
            transform.position = position;

            var temp = CameraPosition.y * (1f - yParallax);

            if (temp > _startPosition.y + _spriteSize.y * 0.5f)
            {
                _startPosition.y += _spriteSize.y;
            }
            else if (temp < _startPosition.y - _spriteSize.y * 0.5f)
            {
                _startPosition.y -= _spriteSize.y;
            }
        }
        
        private void Start()
        {
            _startPosition = transform.position;
            _spriteSize = GetComponent<SpriteRenderer>()?.bounds.size ?? Vector2.zero;
        }

        private void LateUpdate()
        {
            UpdateX();
            
            UpdateY();
        }
    }
}