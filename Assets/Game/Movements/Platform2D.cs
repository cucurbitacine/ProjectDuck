using UnityEngine;

namespace Game.Movements
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Platform2D : MonoBehaviour
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
            Ground2D.AddPlatform(rigidbody2d, this);
        }

        private void OnDisable()
        {
            Ground2D.RemovePlatform(rigidbody2d, out _);
        }
    }
}