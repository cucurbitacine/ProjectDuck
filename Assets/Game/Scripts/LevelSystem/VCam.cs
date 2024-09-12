using Cinemachine;
using UnityEngine;

namespace Game.Scripts.LevelSystem
{
    public static class VCam
    {
        public static CinemachineVirtualCameraBase ActiveCamera { get; private set; }

        private const int MinPriority = 0;
        private const int MaxPriority = 100;
        
        public static CinemachineBrain GetBrain() => Object.FindObjectOfType<CinemachineBrain>();
        
        public static void SetActive(CinemachineVirtualCameraBase camera)
        {
            if (camera == null) return;
            
            if (ActiveCamera)
            {
                ActiveCamera.Priority = MinPriority;
            }

            ActiveCamera = camera;

            if (ActiveCamera)
            {
                ActiveCamera.Priority = MaxPriority;
            }
        }
    }
}
