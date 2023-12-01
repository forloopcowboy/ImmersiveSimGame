using System;
using System.Linq;
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
                    thisNpcState.NPCName = gameObject.name;
                    thisNpcState.Position = transform.position;
                    thisNpcState.Rotation = transform.rotation.eulerAngles;
                    GlobalGameState.State.NPCStates.Add(thisNpcState);
                }
                
                return _state = thisNpcState;
            }
        }
        
        public void LoadState()
        {
            Health.SetHealth(State.Health);
            var navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.Warp(State.Position);
            }
            else
            {
                transform.position = State.Position;
            }
            transform.rotation = Quaternion.Euler(State.Rotation);
            
            Debug.Log($"Loaded NPC state. Health: {Health.currentHealth}");
        }
        
        [Button]
        public void SaveState()
        {
            if (IsMyIdUnique())
            {
                SyncState();
                GlobalGameState.Singleton.SaveState();
            }
            else
            {
                throw new Exception($"Identifier {Identifier} is not unique! Please generate a new one.");
            }
        }

        private void SyncState()
        {
            State.Health = Health.currentHealth;
            State.Position = transform.position;
            State.Rotation = transform.rotation.eulerAngles;
        }

        private void Start()
        {
            LoadState();
        }

        private void Update()
        {
            SyncState();
        }
        
        [Button]
        public void NewIdentifier()
        {
            Identifier = Guid.NewGuid().ToString();
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Identifier))
            {
                Identifier = Guid.NewGuid().ToString();
            }
            
            if (!IsMyIdUnique())
            {
                throw new Exception($"Identifier {Identifier} is not unique! Please generate a new one.");
            }
            
            if (Health == null)
            {
                Health = GetComponent<Health>();
            }
        }

        private bool IsMyIdUnique()
        {
            if (GameManager.GameManager.Singleton == null)
            {
                Debug.LogWarning("GameManager not found. Cannot check for unique ID.");
                return true;
            }
            return GlobalGameState.State.NPCStates.FindAll(state => state.Identifier == Identifier).Count <= 1 && FindObjectsOfType<SerializedNPC>()?.Where(npc => npc.Identifier == Identifier).Count() <= 1;
        }
    }
}