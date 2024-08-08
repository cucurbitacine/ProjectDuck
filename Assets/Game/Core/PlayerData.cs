using System;

namespace Game.Core
{
    [Serializable]
    public class PlayerData
    {
        public string playerName;
        public int levelNumber = -1;
        public bool newGame => levelNumber < 0;

        public override string ToString()
        {
            return $"[{playerName}] Level: {levelNumber}";
        }
    }
}