using System.Collections;
using UnityEngine;

namespace Game.Interactions.Impl
{
    public class DoorController : ToggleBase
    {
        [Header("Door")]
        [Range(0f, 1f)]
        [SerializeField] private float progressBlend = 0;
        
        [Header("Settings")]
        [Min(0f)]
        [SerializeField] private float duration = 1f;
        [SerializeField] private float distance = 2f;
        [SerializeField] private Vector2 direction = Vector2.up;
        
        [Header("References")]
        [SerializeField] private Transform doorAnchor;

        private Coroutine _opening;

        public bool IsOpened => TurnedOn;

        public Vector2 Trajectory => direction.normalized * distance;
        
        public Vector2 ClosedPosition => transform.position;
        public Vector2 OpenedPosition => ClosedPosition + Trajectory;
        
        public void OpenDoor(bool value)
        {
            TurnOn(value);
        }

        [ContextMenu(nameof(OpenDoor))]
        public void OpenDoor()
        {
            OpenDoor(true);
        }
        
        [ContextMenu(nameof(CloseDoor))]
        public void CloseDoor()
        {
            OpenDoor(false);
        }
        
        [ContextMenu(nameof(SwitchDoor))]
        public void SwitchDoor()
        {
            OpenDoor(!IsOpened);
        }
        
        private void SetAnchorPosition(float t)
        {
            doorAnchor.position = Vector2.Lerp(ClosedPosition, OpenedPosition, t);
        }
        
        private void Open(bool value)
        {
            if (_opening != null) StopCoroutine(_opening);
            _opening = StartCoroutine(Opening(value));
        }
        
        private IEnumerator Opening(bool value)
        {
            var originBlend = progressBlend;
            var targetBlend = value ? 1f : 0f;

            var lengthBlend = Mathf.Abs(progressBlend - targetBlend);
            var timeBlend = lengthBlend * duration;
            
            var timer = 0f;
            while (timer < timeBlend)
            {
                if (!Paused)
                {
                    var t = Mathf.SmoothStep(0f, 1f, timer / timeBlend);
                    progressBlend = Mathf.Lerp(originBlend, targetBlend, t);
                
                    SetAnchorPosition(progressBlend);
                }
                
                timer += Time.deltaTime;
                yield return null;
            }

            SetAnchorPosition(targetBlend);
        }
        
        private void OnEnable()
        {
            OnValueChanged += Open;
        }

        private void OnDisable()
        {
            OnValueChanged -= Open;

            if (_opening != null) StopCoroutine(_opening);
            SetAnchorPosition(IsOpened ? 1f : 0f);
        }

        private void Start()
        {
            Open(TurnedOn);
        }

        private void OnValidate()
        {
            if (!Application.isPlaying && Application.isEditor && doorAnchor)
            {
                SetAnchorPosition(progressBlend);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = IsOpened ? Color.green : Color.red;
            Gizmos.DrawLine(ClosedPosition, OpenedPosition);
        }
    }
}