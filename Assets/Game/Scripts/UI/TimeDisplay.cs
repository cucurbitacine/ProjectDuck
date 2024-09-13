using System;
using Game.Scripts.LevelSystem;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class TimeDisplay : MonoBehaviour
    {
        [SerializeField] private TimeDisplayMode timeMode = TimeDisplayMode.PlayerTimeTotal;
        [SerializeField] private PlayerStopwatch stopwatch;
        
        [Space]
        [SerializeField] private TMP_Text time;
        
        private void Update()
        {
            if (timeMode is TimeDisplayMode.PlayerTimeTotal)
            {
                if (stopwatch)
                {
                    time.text = $"{DateTime.FromFileTime(stopwatch.DurationTotalInTicks):HH:mm:ss}";
                }
                else
                {
                    timeMode = TimeDisplayMode.CurrentTime;
                }
            }
            
            if (timeMode == TimeDisplayMode.CurrentTime)
            {
                time.text = $"{DateTime.Now:HH:mm:ss}";
            }
        }
        
        private enum TimeDisplayMode
        {
            CurrentTime,
            PlayerTimeTotal,
        }
    }
}