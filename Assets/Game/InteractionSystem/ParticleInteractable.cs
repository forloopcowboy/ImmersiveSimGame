using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.InteractionSystem
{
    public class ParticleInteractable : InteractableObject
    {
        [InfoBox("This is a particle interactable. It will play all particle systems in the list when interacted with.")]
        public List<ParticleSystem> particleSystems = new();
        
        [Button]
        public void PlayAllParticles()
        {
            foreach (var pSystem in particleSystems)
            {
                pSystem.Play();
            }
        }
        
        [Button]
        public void GetParticlesFromHierarchy()
        {
            particleSystems.Clear();
            particleSystems.AddRange(GetComponentsInChildren<ParticleSystem>());
        }

        protected override void OnInteract(Interactor interactor, object input)
        {
            PlayAllParticles();
        }
    }
}