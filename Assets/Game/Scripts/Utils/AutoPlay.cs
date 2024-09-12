using System;
using UnityEngine;

namespace Game.Scripts.Utils
{
    public class AutoPlay : MonoBehaviour
    {
        [SerializeField] private string stateName = "";
        
        private void OnEnable()
        {
            if (TryGetComponent<Animator>(out var anim))
            {
                anim.Play(stateName);
            }
        }
    }
}
