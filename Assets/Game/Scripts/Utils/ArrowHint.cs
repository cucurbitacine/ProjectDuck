using UnityEngine;

namespace Game.Scripts.Utils
{
    public class ArrowHint : MonoBehaviour
    {
        [SerializeField] private Transform origin;
        [SerializeField] private Transform target;

        [Space]
        [SerializeField] private Vector2 viewportCenter = Vector2.one * 0.5f;
        [SerializeField] [Range(0f, 1f)] private float lerpToCenter = 0.5f;
        
        private static Camera CameraMain => Camera.main;

        private void LateUpdate()
        {
            var originWorldPoint = (Vector2)origin.position;
            var targetWorldPoint = (Vector2)target.position;
            
            var targetViewportPoint = CameraMain.WorldToViewportPoint(targetWorldPoint);
            
            var arrowViewportPoint = targetViewportPoint;
            arrowViewportPoint.x = Mathf.Clamp01(targetViewportPoint.x);
            arrowViewportPoint.y = Mathf.Clamp01(targetViewportPoint.y);
            arrowViewportPoint = Vector2.Lerp(arrowViewportPoint, viewportCenter, lerpToCenter);
            
            var arrowWorldPoint = (Vector2)CameraMain.ViewportToWorldPoint(arrowViewportPoint);

            transform.position = arrowWorldPoint;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, targetWorldPoint - originWorldPoint);
        }
    }
}
