using System.Collections;
using CucuTools.StateMachines;
using Game.Core;
using Game.LevelSystem;
using Game.Player;
using UnityEngine;

namespace Game.Dev
{
    public class Dev_LevelManager : LevelManager
    {
        [Header("Dev")]
        [SerializeField] private PlayerController player;
        [SerializeField] private StateMachineBase stateMachine;

        private IEnumerator LoadPlayer()
        {
            yield return new WaitUntil(() => GameManager.Instance.Player);

            player = GameManager.Instance.Player.GetComponent<PlayerController>();
        }
        
        protected override IEnumerator PrepareLevel()
        {
            yield return LoadPlayer();
            
            player.Pause(true);
            
            //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        protected override void OnStartLevel()
        {
            player.Pause(false);
            
            stateMachine.StartState();
        }

        protected override void OnStopLevel()
        {
        }

        protected override IEnumerator DisposeLevel()
        {
            player.Pause(true);
            
            //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            yield return null;
        }
    }
}
