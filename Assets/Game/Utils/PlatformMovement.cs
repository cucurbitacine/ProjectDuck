using UnityEngine;

namespace Game.Utils
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlatformMovement : MonoBehaviour
    {
        public bool paused = false;

        [Header("Movement")]
        public bool useMovement = true;
        [Min(0f)] public float durationMove = 1f;
        [Min(0f)] public float durationWait = 0f;
        public float initialTime = 0f;

        [Space]
        public bool smoothStep = true;
        public Vector2 offset = Vector2.right;

        [Header("Rotation")]
        public bool useRotation = false;
        public float angularVelocity = 30f;

        [Header("Other")]
        public Vector2 size = Vector2.one;
        
        private Rigidbody2D rigid;
        private float timer;
        
        public float phase { get; private set; }
        [field: SerializeField] public Vector2 origin { get; private set; }
        public Vector2 target => origin + offset;
        
        public float duration => durationMove + durationWait;
        public float distance => offset.magnitude;
        public Vector2 direction => offset.normalized;

        private float EvaluatePhase(float time)
        {
            if (duration > 0f)
            {
                var result = Mathf.PingPong(time, duration) / duration;

                var wait = durationWait / (2 * durationMove);

                var left = 0 - wait;
                var right = 1 + wait;
                
                result = Mathf.Lerp(left, right, result);
                
                if (smoothStep && 0 <= result && result <= 1)
                {
                    result = Mathf.SmoothStep(0, 1, result);
                }
                
                return result;
            }
            
            return 0f;
        }
        
        private Vector2 EvaluatePosition(float t)
        {
            return Vector2.Lerp(origin, target, t);
        }

        private RigidbodyConstraints2D GetConstraints()
        {
            if (paused) return RigidbodyConstraints2D.FreezeAll;

            var result = RigidbodyConstraints2D.None;

            if (!useMovement) result |= RigidbodyConstraints2D.FreezePosition;
            if (!useRotation) result |= RigidbodyConstraints2D.FreezeRotation;

            return result;
        }
        
        private void UpdateMovement()
        {
            if (paused || !useMovement)
            {
                rigid.velocity = Vector2.zero;
            }
            else
            {
                phase = EvaluatePhase(timer + initialTime);
                timer = Mathf.Repeat(timer + Time.fixedDeltaTime, 2 * duration);
            
                var nextPosition = EvaluatePosition(phase);

                rigid.velocity = paused ? Vector2.zero : ((nextPosition - rigid.position) / Time.fixedDeltaTime);
                rigid.MovePosition(nextPosition);
            }
        }

        private void UpdateRotation()
        {
            if (paused || !useRotation)
            {
                rigid.angularVelocity = 0f;
            }
            else
            {
                var nextRotation = rigid.rotation + angularVelocity * Time.fixedDeltaTime;
                
                rigid.angularVelocity = angularVelocity;
                rigid.MoveRotation(nextRotation);
            }
        }
        
        private void UpdateConstraints()
        {
            rigid.constraints = GetConstraints();
        }
        
        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();

            origin = transform.position;
        }
        
        private void FixedUpdate()
        {
            UpdateMovement();

            UpdateRotation();

            UpdateConstraints();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            if (Application.isPlaying)
            {
                Gizmos.DrawWireCube(origin, size);
                Gizmos.DrawWireCube(target, size);
                
                Gizmos.DrawLine(origin, target);
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, size);
            }
            else
            {
                origin = transform.position;
                
                Gizmos.DrawWireCube(origin, size);
                Gizmos.DrawWireCube(target, size);
                
                Gizmos.DrawLine(origin, target);
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(EvaluatePosition(EvaluatePhase(initialTime)), size);
            }
        }
    }
}