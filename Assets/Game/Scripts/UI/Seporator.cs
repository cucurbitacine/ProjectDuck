using UnityEngine;

namespace Game.Scripts.UI
{
    public class Seporator : MonoBehaviour
    {
        [SerializeField] private string title = "Title";

        private void OnValidate()
        {
            gameObject.name = $"==== {title} ====";
        }
    }
}