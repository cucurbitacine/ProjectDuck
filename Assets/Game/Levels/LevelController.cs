using Game.Core;
using UnityEngine;

namespace Game.Levels
{
    public abstract class LevelController : MonoBehaviour
    {
        public static GameManager Game => GameManager.Instance;
    }
}