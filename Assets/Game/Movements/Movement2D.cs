using UnityEngine;

namespace Game.Movements
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Ground2D))]
    public class Movement2D : MonoBehaviour
    {
        [Header("Settings")]
        [Min(0f)] [SerializeField] private float speedMax = 5f;
        [Min(0f)] [SerializeField] private float jumpHeight = 2.5f;
        [Range(0f, 1f)] [SerializeField] private float dragDecay = 0.5f;
        
        [Header("Frictions")]
        [SerializeField] private PhysicsMaterial2D idleFriction;
        [SerializeField] private PhysicsMaterial2D moveFriction;
        
        private Rigidbody2D rigid;
        private bool jumpedOffGround;
        
        public Vector2 input { get; private set; }
        public Ground2D ground { get; private set; }
        public Vector2 groundUp => isGrounded ? ground.groundNormal : worldUp;
        public Vector2 groundRight => -Vector2.Perpendicular(groundUp);
        
        public static Vector2 worldGravity => Physics2D.gravity;
        public static Vector2 worldUp => -worldGravity.normalized;
        public static Vector2 worldRight => -Vector2.Perpendicular(worldUp);
        
        public Vector2 position => transform.position;

        public float mass => rigid ? rigid.mass : 0f;
        public Vector2 gravity => worldGravity * gravityScale;
        public float gravityPower => gravity.magnitude;
        public float gravityScale => rigid ? rigid.gravityScale : 0f;
        
        public Vector2 up => transform.up;
        public Vector2 right => transform.right;
        
        public bool isMoving => !Mathf.Approximately(input.x, 0f);
        public bool isJumping => !isGrounded && Vector2.Dot(Vector3.Project(selfVelocity, worldUp), worldUp) > 0f;
        public bool isFalling => !isGrounded && !isJumping;
        
        public bool isGrounded => ground && ground.isGrounded;
        public bool onSurface => ground && ground.onSurface;
        public bool onSlope => ground && ground.onSlope;
        public bool onPlatform => ground && ground.onPlatform;
        
        public Vector2 velocity
        {
            get => rigid ? rigid.velocity : Vector2.zero;
            private set
            {
                if (rigid) rigid.velocity = value;
            }
        }

        public Vector2 inertialVelocity { get; private set; }
        public Vector2 selfVelocity => velocity - inertialVelocity;
        
        public void Move(Vector2 move)
        {
            input = move;
        }
        
        public void Jump()
        {
            jumpedOffGround = isGrounded;
            
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

        private void UpdateInput(float deltaTime)
        {
            var inputUp = jumpedOffGround ? worldUp : groundUp;
            var inputRight = jumpedOffGround ? worldRight : groundRight;
            
            var inputVelocity = inputRight * (input.x * speedMax);
            
            if (isMoving)
            {
                if (!isGrounded && onSurface)
                {
                    inputVelocity = Vector3.Project(selfVelocity, inputRight);
                }
                
                inputVelocity += (Vector2)Vector3.Project(inertialVelocity, inputRight);
                
                velocity = inputVelocity + (Vector2)Vector3.Project(velocity, inputUp);
            }
        }

        private void UpdateGround(float deltaTime)
        {
            ground.Direction = -up;
            
            ground.CheckGround();

            if (ground.isGrounded)
            {
                inertialVelocity = ground.onPlatform ? ground.groundVelocity : Vector2.zero;
            }
        }
        
        private void UpdateRigid(float deltaTime)
        {
            rigid.sharedMaterial = isGrounded && !isMoving ? idleFriction : moveFriction;

            if (isGrounded && !isMoving && !jumpedOffGround)
            {
                velocity = selfVelocity * dragDecay + inertialVelocity;
            }
        }
        
        private void HandleGround(bool value)
        {
            if (value) 
            {
                // Have Landed
            }
            else 
            {
                // Have Taken Off

                if (jumpedOffGround) jumpedOffGround = false;
            }
        }
        
        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            ground = GetComponent<Ground2D>();

            SetupFrictions();
        }

        private void OnEnable()
        {
            ground.OnChanged += HandleGround;
        }

        private void OnDisable()
        {
            ground.OnChanged -= HandleGround;
        }
        
        private void Update()
        {
            UpdateInput(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            UpdateGround(Time.fixedDeltaTime);
            
            UpdateRigid(Time.fixedDeltaTime);
        }

        private void OnDrawGizmos()
        {
            var groundPoint = onSurface ? ground.groundPoint : position;
                
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundPoint, groundPoint + groundUp);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundPoint, groundPoint + groundRight);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(groundPoint, groundPoint + gravity.normalized);
        }
    }
}