using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class GameVersion : MonoBehaviour
    {
        [SerializeField] private TMP_Text version;

        private void UpdateVersion()
        {
            if (version)
            {
                version.text = $"{Application.platform}_{Application.unityVersion}_{Application.version}";
            }
        }

        private void Awake()
        {
            UpdateVersion();
        }

        private void OnValidate()
        {
            UpdateVersion();
        }
    }
}