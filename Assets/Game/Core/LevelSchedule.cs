using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(menuName = "Create LevelSchedule", fileName = "LevelSchedule", order = 0)]
    public class LevelSchedule : ScriptableObject
    {
        [SerializeField] private List<string> levels = new List<string>();

        public bool TryGetLevelName(int progress, out string level)
        {
            if (progress < 0)
            {
                level = levels[0];
            }
            else if (levels.Count <= progress)
            {
                level = string.Empty;
                return false;
            }
            else
            {
                level = levels[progress];   
            }

            return !string.IsNullOrWhiteSpace(level);
        }
        
        public bool TryGetNextLevelName(int progress, out string nextLevel)
        {
            return TryGetLevelName(progress + 1, out nextLevel);
        }
    }
}