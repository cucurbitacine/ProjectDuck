using System;

namespace Game.Scripts.Core
{
    [Serializable]
    public class PlayerData
    {
        public int playerId;
        public int attemptNumber;
        public int levelNumber = -1;
        public bool newGame => levelNumber < 0;

        public override string ToString()
        {
            return $"[{playerId}:{attemptNumber}] Level: {levelNumber}";
        }
    }
}