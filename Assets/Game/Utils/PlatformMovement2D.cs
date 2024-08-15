using UnityEngine;

namespace Game.Utils
{
    /*
     * SETTINGS
     * 1. Origin position
     * 2. Destination position
     * 3. Current Direction
     * 4. Duration moving
     * 5. Waiting time if looped
     *
     * BEHAVIOUR
     * 1. Only 2 positions     [*]     *
     * 2. Call to move once    [*] --> *
     * 3. Call to move looped  [*] <-> *
     * 4. Call to stop looped   *     [*]
     * 5. If is looped, has wait time on positions
     */

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlatformMovement2D : MonoBehaviour, IPaused
    {
        public enum PlatformState
        {
            Idle,
            Moving,
            Looped,
        }

        [field: SerializeField] public bool Paused { get; private set; }

        [SerializeField] private PlatformState state = PlatformState.Idle;
        
        [field: Space]
        [field: SerializeField] public Vector2 PositionA { get; set; }
        [SerializeField] private float distance = 5f;
        [SerializeField] private Vector2 direction = Vector2.up;
        [SerializeField] private bool reverse = false;
        
        [Space]
        [Min(0)] [SerializeField] private float duration = 5f;
        [Min(0)] [SerializeField] private float waitTime = 1f;
        
        [Space]
        [SerializeField] private Vector2 size = Vector2.one;
        [SerializeField] private Vector2 offset = Vector2.zero;
        
        private Rigidbody2D _rigid;
        private float _waitingTime;
        
        public Vector2 PositionB
        {
            get => PositionA + direction.normalized * distance;
            set
            {
                direction = (value - PositionA).normalized;
                distance = Vector2.Distance(PositionA, value);
            }
        }

        public Vector2 StartPosition => reverse ? PositionB : PositionA;
        public Vector2 DestinationPosition => reverse ? PositionA : PositionB;

        public Vector2 Velocity => _rigid ? _rigid.velocity : Vector2.zero;
        
        public void Pause(bool value)
        {
            Paused = value;
        }

        public void Move(PlatformState newState)
        {
            state = newState;
        }
        
        [ContextMenu(nameof(MoveOnce))]
        public void MoveOnce()
        {
            if (state != PlatformState.Idle) return;

            state = PlatformState.Moving;
        }

        [ContextMenu(nameof(MoveLooped))]
        public void MoveLooped()
        {
            if (state != PlatformState.Idle) return;

            state = PlatformState.Looped;
        }

        [ContextMenu(nameof(StopLoop))]
        public void StopLoop()
        {
            if (state == PlatformState.Looped)
            {
                state = PlatformState.Moving;
            }
        }

        [ContextMenu(nameof(SwitchLoop))]
        public void SwitchLoop()
        {
            if (state == PlatformState.Looped)
            {
                StopLoop();
            }
            else
            {
                MoveLooped();
            }
        }
        
        private RigidbodyConstraints2D GetConstraints()
        {
            return RigidbodyConstraints2D.FreezeRotation;
        }
        
        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _rigid.constraints = GetConstraints();
            _rigid.bodyType = RigidbodyType2D.Kinematic;
            _rigid.interpolation = RigidbodyInterpolation2D.Interpolate;
                
            PositionA = transform.position;
        }

        private void FixedUpdate()
        {
            if (Paused || state == PlatformState.Idle)
            {
                _rigid.velocity = Vector2.zero;
                
                return;
            }
            
            /*
             * Start Position ==> Destination Position
             * Calculation next position
             * Update Rigidbody
             */

            var speed = duration > 0f ? Vector2.Distance(DestinationPosition, StartPosition) / duration : 0f;
            var velocity = (DestinationPosition - _rigid.position).normalized * speed;
                
            var prevPosition = _rigid.position;
            var nextPosition = prevPosition + velocity * Time.fixedDeltaTime;

            var lhs = (prevPosition - DestinationPosition).normalized;
            var rhs = (nextPosition - DestinationPosition).normalized;
            var destinationReached = Vector2.Dot(lhs, rhs) <= 0;
                
            if (destinationReached)
            {
                nextPosition = DestinationPosition;
                reverse = !reverse;
            }
            
            /*
             * Check condition to stop
             * if is looped, start wait
             */

            if (state == PlatformState.Moving)
            {
                _rigid.velocity = velocity;
                _rigid.MovePosition(nextPosition);
                
                if (destinationReached)
                {
                    state = PlatformState.Idle;
                }
            }
            else
            {
                /*
                 * If it should stop, start wait
                 * else Check waiting time
                 */

                if (_waitingTime > 0) 
                {
                    // Is waiting
                    
                    _waitingTime -= Time.fixedDeltaTime;
                    
                    _rigid.velocity = Vector2.zero;
                }
                else
                {
                    // Is moving
                    
                    _rigid.velocity = velocity;
                    _rigid.MovePosition(nextPosition);
                
                    if (destinationReached)
                    {
                        // Start wait
                        _waitingTime = waitTime;
                    }
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) PositionA = transform.position;

            if (TryGetComponent<BoxCollider2D>(out var box))
            {
                size = box.size;
                offset = box.offset;
            }
            
            Gizmos.color = Color.magenta;

            Gizmos.DrawLine(StartPosition + offset, DestinationPosition + offset);
            Gizmos.DrawWireCube(StartPosition + offset, size);
            Gizmos.DrawWireCube(DestinationPosition + offset, size);
        }
    }
}