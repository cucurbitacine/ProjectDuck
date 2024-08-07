using Game.Combat;
using Game.LevelSystem;
using Game.Player;
using UnityEngine;

namespace Game.UI
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBar;

        private void HandlePlayer(PlayerController player)
        {
            healthBar.SetHealth(player?.Health);
        }

        private void Awake()
        {
            if (healthBar == null) healthBar = GetComponentInChildren<HealthBar>();
        }

        private void OnEnable()
        {
            LevelManager.OnPlayerChanged += HandlePlayer;
        }

        private void OnDisable()
        {
            LevelManager.OnPlayerChanged -= HandlePlayer;
        }
    }
}