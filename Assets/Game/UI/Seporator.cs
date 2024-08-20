using System;
using UnityEngine;

namespace Game.UI
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