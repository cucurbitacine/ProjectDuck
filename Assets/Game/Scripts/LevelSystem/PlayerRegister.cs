using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.LevelSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerController))]
    public class PlayerRegister : MonoBehaviour
    {
        private PlayerController _player;
        
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }
        
        private void OnEnable()
        {
            LevelManager.SetPlayer(_player);
        }

        private void OnDisable()
        {
            LevelManager.RemovePlayer();
        }
    }
}