using UnityEngine;

namespace Game.Movements
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Ground2D))]
    public class Movement2D : MonoBehaviour
    {
        public enum MovementState
        {
            Grounded,
            InAir
        }
        
        [field: SerializeField] public MovementState State { get; private set; }
        
        [Header("Settings")]
        [Min(0f)] [SerializeField] private float speedMax = 5f;
        [Min(0f)] [SerializeField] private float jumpHeight = 2.5f;
        [Range(0f, 1f)] [SerializeField] private float dragDecay = 0.5f;
        
        [Header("Frictions")]
        [SerializeField] private PhysicsMaterial2D idleFriction;
        [SerializeField] private PhysicsMaterial2D moveFriction;
        
        private Rigidbody2D _rigidbody2d;
        private bool _jumpedOffGround;
        
        public Vector2 move { get; private set; }
        public Vector2 groundUp => isGrounded ? Ground2D.groundNormal : worldUp;
        public Vector2 groundRight => -Vector2.Perpendicular(groundUp);
        
        public Ground2D Ground2D { get; private set; }
        
        public static Vector2 worldGravity => Physics2D.gravity;
        public static Vector2 worldUp => -worldGravity.normalized;
        public static Vector2 worldRight => -Vector2.Perpendicular(worldUp);
        
        public Vector2 position => transform.position;
        
        public float mass => _rigidbody2d ? _rigidbody2d.mass : 0f;
        public Vector2 gravity => worldGravity * gravityScale;
        public float gravityPower => gravity.magnitude;
        public float gravityScale
        {
            get => _rigidbody2d ? _rigidbody2d.gravityScale : 0f;
            set
            {
                if (_rigidbody2d)
                {
                    _rigidbody2d.gravityScale = value;
                }
            }
        }

        public Vector2 up => transform.up;
        public Vector2 right => transform.right;
        
        public bool isMoving => !Mathf.Approximately(move.x, 0f);
        public bool isJumping => !isGrounded && Vector2.Dot(Vector3.Project(selfVelocity, worldUp), worldUp) > 0f;
        public bool isFalling => !isGrounded && !isJumping;
        
        public bool isGrounded => Ground2D && Ground2D.isGrounded;
        public bool onSurface => Ground2D && Ground2D.onSurface;
        public bool onSlope => Ground2D && Ground2D.onSlope;
        public bool onInertialGround => Ground2D && Ground2D.isInertial;
        
        public Vector2 velocity
        {
            get => _rigidbody2d ? _rigidbody2d.velocity : Vector2.zero;
            private set
            {
                if (_rigidbody2d)
                {
                    _rigidbody2d.velocity = value;
                }
            }
        }

        public Vector2 inertialVelocity { get; private set; }
        public Vector2 selfVelocity => velocity - inertialVelocity;

        public void Warp(Vector2 newPosition)
        {
            if (_rigidbody2d)
            {
                _rigidbody2d.position = newPosition;
                
                _rigidbody2d.velocity = Vector2.zero;
            }
        }
        
        public void Move(Vector2 input)
        {
            move = input;
        }
        
        public void Jump()
        {
            _jumpedOffGround = isGrounded;
            
            var jumpSpeed = Mathf.Sqrt(2 * gravityPower * jumpHeight);
            var jumpVelocity = worldUp * jumpSpeed;

            if (isGrounded)
            {
                jumpVelocity += (Vector2)Vector3.Project(inertialVelocity, worldUp);
            }
            else
            {
                inertialVelocity = Vector2.zero;
            }
            
            velocity = jumpVelocity + (Vector2)Vector3.Project(velocity, groundRight);
        }
        
        private void SetupFrictions()
        {
            if (idleFriction == null)
            {
                idleFriction = new PhysicsMaterial2D()
                {
                    name = nameof(idleFriction),
                    friction = 10f,
                    bounciness = 0f,
                };
            }
            
            if (moveFriction == null)
            {
                moveFriction = new PhysicsMaterial2D()
                {
                    name = nameof(moveFriction),
                    friction = 0f,
                    bounciness = 0f,
                };
            }
        }
        
        private void UpdateMovement(float deltaTime)
        {
            if (!isMoving) return;
            
            var movementUp = _jumpedOffGround ? worldUp : groundUp;
            var movementRight = _jumpedOffGround ? worldRight : groundRight;
            
            var movementVelocity = movementRight * (move.x * speedMax);
            
            // We should slide
            if (onSurface && !isGrounded)
            {
                // Keep velocity along axis Right
                movementVelocity = Vector3.Project(selfVelocity, movementRight);
            }
                
            // Apply inertial velocity along axis Right
            movementVelocity += (Vector2)Vector3.Project(inertialVelocity, movementRight);
                
            // Keep velocity along axis Up
            velocity = movementVelocity + (Vector2)Vector3.Project(velocity, movementUp);
        }

        private void UpdateGround(float deltaTime)
        {
            Ground2D.Direction = -up;
            
            Ground2D.CheckGround();

            if (Ground2D.isGrounded)
            {
                inertialVelocity = Ground2D.isInertial ? Ground2D.groundVelocity : Vector2.zero;
            }
        }
        
        private void UpdateRigidbody(float deltaTime)
        {
            _rigidbody2d.sharedMaterial = isGrounded && !isMoving ? idleFriction : moveFriction;

            if (isGrounded && !isMoving && !_jumpedOffGround)
            {
                velocity = selfVelocity * dragDecay + inertialVelocity;
            }
        }
        
        private void HandleGround(bool value)
        {
            if (value) 
            {
                // Have Landed
                
                State = MovementState.Grounded;
            }
            else 
            {
                // Have Taken Off

                State = MovementState.InAir;
                
                if (_jumpedOffGround) _jumpedOffGround = false;
            }
        }
        
        private void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
            
            Ground2D = GetComponent<Ground2D>();

            SetupFrictions();
        }

        private void OnEnable()
        {
            Ground2D.OnGrounded += HandleGround;
        }

        private void OnDisable()
        {
            Ground2D.OnGrounded -= HandleGround;
        }

        private void Update()
        {
            UpdateMovement(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            UpdateGround(Time.fixedDeltaTime);
            
            UpdateRigidbody(Time.fixedDeltaTime);
        }

        private void OnDrawGizmos()
        {
            var groundPoint = onSurface ? Ground2D.groundPoint : position;
                
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundPoint, groundPoint + groundUp);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundPoint, groundPoint + groundRight);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(groundPoint, groundPoint + gravity.normalized);
        }
    }
}