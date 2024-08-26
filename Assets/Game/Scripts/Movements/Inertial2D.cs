using UnityEngine;

namespace Game.Scripts.Movements
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Inertial2D : MonoBehaviour
    {
        [SerializeField] private bool paused = false;
        
        public Rigidbody2D rigidbody2d { get; private set; }

        public bool active => !paused;

        private void Awake()
        {
            rigidbody2d = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            Ground2D.AddInertial(rigidbody2d, this);
        }

        private void OnDisable()
        {
            Ground2D.RemoveInertial(rigidbody2d, out _);
        }
    }
}