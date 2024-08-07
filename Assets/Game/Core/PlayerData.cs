using System;

namespace Game.Core
{
    [Serializable]
    public class PlayerData
    {
        public string playerName;
        public int currentLevel = -1;
        public bool newGame => currentLevel < 0;
    }
}