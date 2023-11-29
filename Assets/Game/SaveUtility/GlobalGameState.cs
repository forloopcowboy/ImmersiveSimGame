using System;
using System.Collections.Generic;
using System.IO;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.SaveUtility
{
    /// <summary>
    /// Stores game state.
    /// Responsible for saving and loading game state.
    /// </summary>
    public class GlobalGameState : SingletonMonoBehaviour<GlobalGameState>
    {
        [TabGroup("General")]
        public string FileName = "GameState.json";

        [ShowInInspector, ReadOnly, TabGroup("General")]
        public string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        private bool _loaded = false;
        
        [SerializeField]
        private GameState _gameState = new();

        public static GameState State
        {
            get
            {
                if (Singleton._loaded)
                {
                    return Singleton._gameState;
                }
                
                Singleton.LoadState();
                Singleton._loaded = true;
                return Singleton._gameState;
            }
        }
        
        private void Awake()
        {
            LoadState();
        }

        [Button]
        public void InitializeGameState()
        {
            if (_gameState == null)
            {
                _gameState = new GameState();
            }
            
            // Get current scene index
            _gameState.CurrentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
                
            // Initialize level states
            for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                _gameState.LevelStates.Add(new LevelState(i, new List<string>(), new List<NPCState>()));
            }
        }

        [Button]
        public void SaveState()
        {
            var json = JsonUtility.ToJson(_gameState);
            File.WriteAllText(FilePath, json);
            
            Debug.Log($"Saved game state [{FileName}]. Current level: {_gameState.CurrentLevel}");
        }
        
        [Button]
        public void LoadState()
        {
            if (!File.Exists(FilePath))
            {
                Debug.Log($"No save file found at {FilePath}.");
                return;
            }
            
            var json = File.ReadAllText(FilePath);
            _gameState = JsonUtility.FromJson<GameState>(json);
            
            if (_gameState == null || _gameState.CurrentLevel == -1)
            {
                Debug.Log($"No current level found in save file {FilePath}. Initializing game state.");
                InitializeGameState();
            }

            Debug.Log($"Loaded game state [{FileName}]. Current level: {_gameState.CurrentLevel}");
        }
    }

    [Serializable]
    public class GameState
    {
        public int CurrentLevel = -1;
        public List<LevelState> LevelStates = new();
        public PlayerState PlayerState = new();
        
        public List<NPCState> NPCStates => LevelStates[CurrentLevel].NPCStates;

        public Vector3 PlayerPosition
        {
            get => LevelStates[CurrentLevel].PlayerPosition;
            set => LevelStates[CurrentLevel].PlayerPosition = value;
        }
        
        public Vector3 PlayerRotation
        {
            get => LevelStates[CurrentLevel].PlayerRotation;
            set => LevelStates[CurrentLevel].PlayerRotation = value;
        }
    }
}