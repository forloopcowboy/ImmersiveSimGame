using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.ProjectileSystem
{
    public class ParticleSystemController : SerializedMonoBehaviour
    {
        [Required]
        public ParticleSystem targetParticleSystem;

        private void OnValidate()
        {
            if (!targetParticleSystem)
                targetParticleSystem = GetComponentInChildren<ParticleSystem>();
        }
        
        public void SetEmissionRateMultiplier(float multiplier)
        {
            var emission = targetParticleSystem.emission;
            emission.rateOverTimeMultiplier = multiplier;
        }
    }
}