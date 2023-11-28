using System;
using Game.HealthSystem;
using Game.SaveUtility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.AIBehavior
{
    /// <summary>
    /// Handles loading and saving of AI behavior.
    /// </summary>
    public class SerializedNPC : SerializedMonoBehaviour
    {
        public string Identifier = "none";
        
        [TabGroup("Components")] public Health Health; 
        
        private NPCState _state = null;
        public NPCState State
        {
            get
            {
                if (_state != null)
                {
                    return _state;
                }
                
                OnValidate();
                
                var thisNpcState = GlobalGameState.State.NPCStates.Find(state => state.Identifier == Identifier);
                if (thisNpcState == null)
                {
                    thisNpcState = new NPCState();
                    thisNpcState.Identifier = Identifier;
                    thisNpcState.Health = Health.currentHealth;
                    GlobalGameState.State.NPCStates.Add(thisNpcState);
                }
                
                return _state = thisNpcState;
            }
        }
        
        public void LoadState()
        {
            Health.SetHealth(State.Health);
            Debug.Log($"Loaded NPC state. Health: {Health.currentHealth}");
        }
        
        [Button]
        public void SaveState()
        {
            State.Health = Health.currentHealth;
        }

        private void Start()
        {
            LoadState();
        }

        private void Update()
        {
            SaveState();
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Identifier) || Identifier == "none")
            {
                Identifier = Guid.NewGuid().ToString();
            }
            
            if (Health == null)
            {
                Health = GetComponent<Health>();
            }
        }
    }
}