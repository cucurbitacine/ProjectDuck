using System;
using UnityEngine;

namespace Game.Utils
{
    public class ActiveOnStart : MonoBehaviour
    {
        [SerializeField] private bool expectedActive = false;

        private void Start()
        {
            gameObject.SetActive(expectedActive);
        }
    }
}
