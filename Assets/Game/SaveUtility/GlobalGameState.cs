using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField]
        private PlayerSpawnState _playerSpawnState = new();

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

        public static PlayerSpawnState SpawnState
        {
            get => Singleton._playerSpawnState;
            set => Singleton._playerSpawnState = value;
        }
        
        protected void Start()
        {
            if (Singleton == this)
            {
                DontDestroyOnLoad(gameObject);
                SceneManager.activeSceneChanged += HandleSceneChanged;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveState();
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                LoadState();
                GameManager.GameManager.Singleton.ReloadScene();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.activeSceneChanged -= HandleSceneChanged;
        }

        private void HandleSceneChanged(Scene arg0, Scene arg1)
        {
            _gameState.CurrentLevel = arg1.buildIndex;
            Debug.Log("Scene changed. Current level: " + _gameState.CurrentLevel);
        }

        [Button]
        public void InitializeGameState()
        {
            Debug.Log("Initializing game state...");
            if (_gameState == null)
            {
                _gameState = new GameState();
            }
            
            // Spawn at default spawn position
            SpawnState.RestoreLastPosition = false;
            SpawnState.SpawnID = null;
            
            // Get current scene index
            _gameState.CurrentLevel = SceneManager.GetActiveScene().buildIndex;
            Debug.Log("> Current level: " + _gameState.CurrentLevel);
                
            // Initialize level states
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                _gameState.LevelStates.Add(new LevelState(i, new List<SerializedEvent>(), new List<NPCState>(), new List<GameItemState>()));
            }
            Debug.Log("> Initialized " + SceneManager.sceneCountInBuildSettings + " level states.");
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
            else {
                // When loading the state we want to restore where the player was, not where they spawned
                SpawnState.RestoreLastPosition = true;
            }
            
            if (_gameState.CurrentLevel != SceneManager.GetActiveScene().buildIndex)
            {
                Debug.Log($"Current level in save file {FilePath} does not match current scene. Loading correct level.");
                GameManager.GameManager.Singleton.LoadScene(_gameState.CurrentLevel);
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

        /// <summary>
        /// Loads all level events.
        /// </summary>
        public List<SerializedEvent> QuestEvents => PlayerState.Events;
        
        
        public Vector3 PlayerPosition
        {
            get => LevelStates[CurrentLevel].PlayerPosition;
            set {
                LevelStates[CurrentLevel].PlayerLocationInitialized = true;
                LevelStates[CurrentLevel].PlayerPosition = value;
            }
        }
        
        public Vector3 PlayerRotation
        {
            get => LevelStates[CurrentLevel].PlayerRotation;
            set {
                LevelStates[CurrentLevel].PlayerLocationInitialized = true;
                LevelStates[CurrentLevel].PlayerRotation = value;
            }
        }

        public LevelState CurrentLevelState => LevelStates[CurrentLevel];
        
        public bool HasNpcEventOccurred(string npcId, Predicate<SerializedEvent> predicate)
        {
            return LevelStates[CurrentLevel].HasNpcEventOccurred(npcId, predicate);
        }
        
        public void RecordNpcEvent(string npcId, SerializedEvent serializedEvent)
        {
            LevelStates[CurrentLevel].RecordNpcEvent(npcId, serializedEvent);
        }
    }
    
    [Serializable]
    public class PlayerSpawnState
    {
        public bool RestoreLastPosition;
        public string SpawnID;
    }
}