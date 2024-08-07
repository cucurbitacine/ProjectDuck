using System.Collections;
using CucuTools.StateMachines;
using Game.LevelSystem;
using UnityEngine;

namespace Game.Dev
{
    public class DevLevelManager : LevelManager
    {
        [Header("Dev")]
        [SerializeField] private StateMachineBase stateMachine;

        private static IEnumerator WaitPlayer()
        {
            yield return new WaitUntil(() => Player);
        }
        
        protected override IEnumerator PrepareLevel()
        {
            yield return WaitPlayer();
            
            Player.Pause(true);
            
            //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        protected override void OnStartLevel()
        {
            Player.Pause(false);
            
            stateMachine.StartState();
        }

        protected override void OnStopLevel()
        {
        }

        protected override IEnumerator DisposeLevel()
        {
            Player.Pause(true);
            
            //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            yield return null;
        }
    }
}
