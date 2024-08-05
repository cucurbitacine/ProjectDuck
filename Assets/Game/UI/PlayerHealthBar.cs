using Game.Combat;
using Game.Core;
using UnityEngine;

namespace Game.UI
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBar;

        private void HandlePlayer(GameObject playerGameObject)
        {
            var health = playerGameObject?.GetComponent<Health>();
                    
            healthBar.SetHealth(health);
        }

        private void Awake()
        {
            if (healthBar == null) healthBar = GetComponentInChildren<HealthBar>();
        }

        private void OnEnable()
        {
            GameManager.Instance.OnPlayerChanged += HandlePlayer;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnPlayerChanged -= HandlePlayer;
        }
    }
}