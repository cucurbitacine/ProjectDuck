using System;
using UnityEngine;

namespace Game.Scripts.SFX
{
    [Serializable]
    public class AudioSettings
    {
        public SpatialType spatialType = SpatialType.Surrounded;

        [Header("Surrounded Settings Only")] 
        [Range(0f, 1f)] public float spreadBlend = 0.8f;
        
        [Space]
        public RolloffType rolloffType = RolloffType.Linear;
        [Range(0f, 5f)] public float dopplerLevel = 0f;
        
        [Space]
        [Min(0f)] public float minDistance = 5f;
        [Min(0f)] public float maxDistance = 20f;

        public AudioRolloffMode rolloffMode => rolloffType == RolloffType.Linear
            ? AudioRolloffMode.Linear
            : AudioRolloffMode.Logarithmic;
        
        public float spatialBlend => spatialType == SpatialType.Surrounded ? 1f : 0f;
        public float spread => Mathf.Lerp(0f, 180f, spreadBlend);

        public void Copy(AudioSettings settings)
        {
            spatialType = settings.spatialType;
            
            spreadBlend = settings.spreadBlend;
            
            rolloffType = settings.rolloffType;
            dopplerLevel = settings.dopplerLevel;
            
            minDistance = settings.minDistance;
            maxDistance = settings.maxDistance;
        }
    }
    
    public enum SpatialType
    {
        Flat,
        Surrounded, 
    }

    public enum RolloffType
    {
        Linear,
        Logarithmic, 
    }
}