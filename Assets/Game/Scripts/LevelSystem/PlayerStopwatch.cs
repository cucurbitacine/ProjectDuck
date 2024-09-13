using System;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.LevelSystem
{
    public class PlayerStopwatch : MonoBehaviour
    {
        [SerializeField] private bool isRunning = false;
        [SerializeField] private bool isDataLoaded = false;
        [SerializeField] private PlayerData playerData;

        [field: Space]
        [field: SerializeField] public long DurationInTicks { get; private set; }

        public long DurationTotalInTicks => DurationInTicks + (isDataLoaded ? playerData.time : 0);
        
        private DateTime startTime;
        
        public void StartStopwatch()
        {
            isRunning = true;

            DurationInTicks = 0;
            
            startTime = DateTime.Now;
        }
        
        public void StopStopwatch()
        {
            isRunning = false;
        }

        public void SaveRecord()
        {
            if (isDataLoaded)
            {
                playerData.time += DurationInTicks;
                
                DurationInTicks = 0;
            }
        }
        
        private async void Start()
        {
            playerData = await GameManager.Instance.GetPlayerDataAsync();

            isDataLoaded = true;
        }

        private void Update()
        {
            if (isDataLoaded && isRunning)
            {
                DurationInTicks = DateTime.Now.Ticks - startTime.Ticks;
            }
        }
    }
}
