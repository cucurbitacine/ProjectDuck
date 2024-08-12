using System;
using System.Collections.Generic;
using CucuTools.StateMachines;
using Game.Player;
using UnityEngine;

namespace Game.Dev
{
    public class DevLevelControl : MonoBehaviour
    {
        public PlayerController player;

        [Space]
        public StateMachineBase stateMachine;
        public int roomNumber = 0;
        public List<RoomData> rooms = new List<RoomData>();

        private void Select(RoomData room)
        {
            if (stateMachine && room.complete)
            {
                stateMachine.SetNextState(room.complete);
            }
            
            room.complete?.Done();
                
            if (room.spawn)
            {
                player.GetMovement2D().Warp(room.spawn.position);
            }
        }
        
        private void Next()
        {
            if (!stateMachine || !stateMachine.isActive) return;
            
            if (roomNumber < 0 || rooms.Count <= roomNumber) return;
            
            var room = rooms[roomNumber];
            
            if (room == null) return;

            if (room.complete && room.complete.isDone)
            {
                roomNumber++;
                
                Next();
            }
            else
            {
                Select(room);
                
                roomNumber++;
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                Next();
            }
        }
    }

    [Serializable]
    public class RoomData
    {
        public StateBase complete;
        
        [Space]
        public Transform spawn;
    }

    public static class StateExt
    {
        public static void Done(this StateBase state)
        {
            state.isDone = true;
        }
    }
}