using System;

namespace Game.Scripts.Core
{
    /// <summary>
    /// Player Data Info
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public int playerId;
        public int attemptNumber;
        public int levelNumber = -1;
        
        public override string ToString()
        {
            return $"[{playerId}:{attemptNumber}] Level: {levelNumber}";
        }
    }
}