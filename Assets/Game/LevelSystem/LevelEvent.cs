using UnityEngine;
using UnityEngine.Events;

namespace Game.LevelSystem
{
    [RequireComponent(typeof(LevelManager))]
    public sealed class LevelEvent : MonoBehaviour
    {
        [SerializeField] private LevelManager.LevelResult expectedResult = LevelManager.LevelResult.Won;
        [Space]
        [SerializeField] private UnityEvent<LevelManager.LevelResult> levelEvent = new UnityEvent<LevelManager.LevelResult>();
        
        private LevelManager _level;

        private void HandleLevel(LevelManager.LevelResult result)
        {
            if (expectedResult == result)
            {
                levelEvent.Invoke(result);
            }
        }
        
        private void Awake()
        {
            _level = GetComponent<LevelManager>();
        }
        
        private void OnEnable()
        {
            _level.OnLevelEnded += HandleLevel;
        }

        private void OnDisable()
        {
            _level.OnLevelEnded -= HandleLevel;
        }
    }
}