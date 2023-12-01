using System;
using System.Collections.Generic;
using System.Linq;
using Game.DialogueSystem;
using Game.EquipmentSystem;
using Game.GameManager;
using Game.GrabSystem;
using Game.HealthSystem;
using Game.InteractionSystem;
using Game.ProgressBar;
using Game.SaveUtility;
using Game.Utils;
using KinematicCharacterController.Examples;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KinematicCharacterController.ExampleCharacter.Scripts
{
    public class PlayerSpawner : SerializedMonoBehaviour
    {
        [ReadOnly]
        public SpawnPoint[] spawnPoints;
        [Required("Spawn point is required to spawn player. If none is provided, spawner position will be used.", InfoMessageType.Warning)]
        public SpawnPoint defaultSpawnPoint;
        
        public GameObject playerPrefab;
        public GameObject playerCharacterPrefab;
        public GameObject playerCameraPrefab;
        public GameObject gameUIPrefab;
        public GameManager gameManagerPrefab;

        public bool spawnOnStart = true;
        
        public PlayerState _playerState;
        
        // Internal state and instances
        [ReadOnly]
        private KinematicPlayer _kinematicPlayerInstance;
        [ReadOnly]
        private DialogueSystem _dialogueSystemInstance;
        [ReadOnly]
        private DeathScreenUIController _deathScreenUIControllerInstance;
        [ReadOnly]
        private HealthBar _playerHealthBarInstance;

        [Button(ButtonSizes.Small)]
        private void OnValidate()
        {
            spawnPoints = FindObjectsOfType<SpawnPoint>();
        }

        private void Awake()
        {
            OnValidate();
            
            if (spawnOnStart)
            {
                SpawnPlayer();
            }
        }

        [Button]
        public void SpawnPlayer()
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            var player = Instantiate(playerPrefab, position, rotation);
            player.name = "Player Master";
            
            var playerCharacter = Instantiate(playerCharacterPrefab, position, rotation);
            playerCharacter.name = "Player KinematicCharacter";
            
            var playerCamera = Instantiate(playerCameraPrefab, position, rotation);
            playerCamera.name = "KinematicCamera";

            var kinematicPlayer = player.GetComponentInChildren<KinematicPlayer>();
            var kinematicCharacter = playerCharacter.GetComponentInChildren<Examples.KinematicCharacterController>();
            var kinematicCamera = playerCamera.GetComponentInChildren<KinematicCharacterCamera>();
            
            // check for nulls
            if (kinematicPlayer == null)
            {
                Debug.LogError("PlayerSpawner: Could not find KinematicPlayer component in playerPrefab!");
                return;
            }
            if (kinematicCharacter == null)
            {
                Debug.LogError("PlayerSpawner: Could not find KinematicCharacterController component in playerCharacterPrefab!");
                return;
            }
            if (kinematicCamera == null)
            {
                Debug.LogError("PlayerSpawner: Could not find KinematicCharacterCamera component in playerCameraPrefab!");
                return;
            }
            
            _kinematicPlayerInstance = kinematicPlayer;
            
            InitializePlayer(kinematicPlayer, kinematicCharacter, kinematicCamera);
            InitializeUI(kinematicPlayer);
            
            LoadPlayerState();
        }

        [Button]
        private void SavePlayerState()
        {
            SyncPlayerState();
            
            GlobalGameState.Singleton.SaveState();
        }

        [Button]
        private void LoadPlayerState()
        {
            LoadPlayerState(_kinematicPlayerInstance);
        }
        
        private void LoadPlayerState(KinematicPlayer kinematicPlayer)
        {
            _playerState = GlobalGameState.State.PlayerState;
            
            var inventory = kinematicPlayer.Character.GetComponentInChildren<GameItemInventory>();
            var health = kinematicPlayer.Character.GetComponentInChildren<Health>();

            if (_playerState == null || !_playerState.IsInitialized)
            {
                Debug.Log("Player state is not initialized. Initializing...");
                _playerState = GlobalGameState.State.PlayerState = new PlayerState();
                GlobalGameState.State.PlayerState.IsInitialized = true;
                
                SyncPlayerState();
                GlobalGameState.State.PlayerState.EquippedItems = inventory.EquippedItems = new []{-1, -1, -1, -1 , -1, -1, -1, -1, -1, -1};
            }
            else
            {
                Debug.Log("Player state is initialized. Loading...");
                health.SetHealth(_playerState.Health);
                
                var playerStateInventory = _playerState.Inventory;
                inventory.ItemsInInventory = new List<GameItemInInventory>();
                
                foreach (var serializedItemData in playerStateInventory)
                {
                    if (ItemDatabase.TryGetItem<GameItemType>(serializedItemData.Identifier, out var item))
                    {
                        Debug.Log($"Restoring item {item.ItemName} ({item.Identifier})");
                        inventory.ItemsInInventory.Add(new GameItemInInventory {Item = item, Quantity = serializedItemData.Quantity, Inventory = inventory});
                    }
                    else
                    {
                        Debug.LogWarning($"Item with identifier {serializedItemData.Identifier} could not be found in the item database.");
                    }
                }

                inventory.EquippedItems = _playerState.EquippedItems;
                inventory.activelyHeldItem = _playerState.HeldItemIndex >= 0 && _playerState.HeldItemIndex < inventory.ItemsInInventory.Count ? inventory.ItemsInInventory[_playerState.HeldItemIndex] : null;
            }

            Vector3 position;
            Quaternion rotation;
            
            if (GlobalGameState.SpawnState.RestoreLastPosition)
            {
                position = GlobalGameState.State.PlayerPosition;
                rotation = Quaternion.Euler(GlobalGameState.State.PlayerRotation);
            }
            else
            {
                var spawnIdFromStateIsEmpty = String.IsNullOrEmpty(GlobalGameState.SpawnState.SpawnID);
                var spawnPoint = spawnIdFromStateIsEmpty
                    ? defaultSpawnPoint
                    : spawnPoints.FirstOrDefault(sp => sp.SpawnID == GlobalGameState.SpawnState.SpawnID);

                Transform spawnPointTransform = spawnPoint == null ? transform : spawnPoint.transform;

                Debug.Log($"PlayerSpawner: Spawn point is {spawnPointTransform.name} (ID: {spawnPoint?.SpawnID ?? "null"})");
                
                position = spawnPointTransform.position;
                rotation = spawnPointTransform.rotation;
            }
            
            // Check if player location should be loaded or initialized
            _kinematicPlayerInstance.Character.Motor.enabled = false;
            _kinematicPlayerInstance.Character.transform.SetPositionAndRotation(position,rotation);
            _kinematicPlayerInstance.Character.Motor.SetPositionAndRotation(position,rotation);
            
            StartCoroutine(CoroutineHelpers.DelayedAction(0.5f, () =>
            {
                SyncPlayerPosition(); // we loaded position, so put it in the state
                _kinematicPlayerInstance.Character.Motor.enabled = true;
            }));
        }

        private void InitializeUI(KinematicPlayer kinematicPlayer)
        {
            var gameUI = Instantiate(gameUIPrefab);
            var gameManager = GameManager.Singleton != null ? GameManager.Singleton : Instantiate(gameManagerPrefab);
            gameManager.name = "Game Manager";

            var health = kinematicPlayer.Character.GetComponentInChildren<Health>();
            gameUI.name = "Game User Interface";

            // initialize player HUD
            var deathScreenUI = gameUI.GetComponentInChildren<DeathScreenUIController>();
            if (deathScreenUI != null)
            {
                var respawnBtn = deathScreenUI.GetComponentInChildren<Button>(true);
                respawnBtn.onClick.AddListener(() =>
                {
                    // Load previous save
                    GlobalGameState.Singleton.LoadState();
                    
                    // Restart scene
                    GameManager.Singleton.ReloadScene();
                    
                    // Hide death screen
                    deathScreenUI.root.gameObject.SetActive(false);
                });
                
                deathScreenUI.health = health;
            }
            var playerHealthBar = gameUI.GetComponentInChildren<HealthBar>();
            if (playerHealthBar != null)
                playerHealthBar.health = health;
            
            // Initialize actions UI
            var actionUI = gameUI.GetComponentInChildren<InteractableActionUIController>();
            var interactor = kinematicPlayer.Character.GetComponentInChildren<Interactor>();
            
            if (actionUI != null)
                actionUI.Interactor = interactor;
            
            // initialize inventory UI
            var inventoryUI = gameUI.GetComponentInChildren<InventoryContentUIController>();
            var equippedUI = gameUI.GetComponentInChildren<EquipmentThumbnailUIController>();
            var inventory = kinematicPlayer.Character.GetComponentInChildren<GameItemInventory>();
            
            if (inventoryUI != null)
                inventoryUI.Inventory = inventory;
            if (equippedUI != null)
                equippedUI.Inventory = inventory;
            kinematicPlayer.InventoryContentUIController = inventoryUI;
            
            _dialogueSystemInstance = gameUI.GetComponentInChildren<DialogueSystem>();
            _deathScreenUIControllerInstance = deathScreenUI;
            _playerHealthBarInstance = playerHealthBar;

            // Handle game manager state
            gameManager.onPause.AddListener(HideUIOnPause);
            gameManager.onResume.AddListener(ShowUIOnResume);
        }

        private void HideUIOnPause()
        {
            if (_dialogueSystemInstance != null) _dialogueSystemInstance.gameObject.SetActive(false);
            if (_deathScreenUIControllerInstance != null) _deathScreenUIControllerInstance.gameObject.SetActive(false);
            if (_playerHealthBarInstance != null) _playerHealthBarInstance.Hide();
        }
        
        private void ShowUIOnResume()
        {
            _dialogueSystemInstance.gameObject.SetActive(true);
            _deathScreenUIControllerInstance.gameObject.SetActive(true);
            _playerHealthBarInstance.Show();
        }

        private void InitializePlayer(KinematicPlayer kinematicPlayer, Examples.KinematicCharacterController kinematicCharacter, KinematicCharacterCamera kinematicCamera)
        {
            kinematicPlayer.Character = kinematicCharacter;
            kinematicPlayer.CharacterCamera = kinematicCamera;
            
            kinematicPlayer.OnValidate();

            var grabNodePosition = kinematicCamera.transform.FindTransformByName("GrabNodePosition");
            var grabNode = kinematicCharacter.GetComponentInChildren<GrabNode>();
            
            if (grabNodePosition == null || grabNode == null)
            {
                if (grabNodePosition == null) Debug.LogError("PlayerSpawner: Could not find GrabNodePosition transform in playerCameraPrefab!");
                if (grabNode == null) Debug.LogError("PlayerSpawner: Could not find GrabNode component in playerCharacterPrefab!");
            }
            else
            {
                var grabNodePositionFollower = grabNode.GetOrElseAddComponent<FollowWorldTransform>();
                grabNodePositionFollower.target = grabNodePosition;
                grabNodePositionFollower.followPosition = true;
                grabNodePositionFollower.followRotation = true;
            }
            
            var projectileSpawnPoint = kinematicCharacter.transform.FindTransformByName("ProjectileSpawnPoint");
            if (projectileSpawnPoint == null)
            {
                Debug.LogError("PlayerSpawner: Could not find ProjectileSpawnPoint transform in playerCharacterPrefab!");
            }
            else
            {
                var projectileSpawnPointFollower = projectileSpawnPoint.GetOrElseAddComponent<FollowWorldTransform>();
                projectileSpawnPointFollower.target = kinematicCamera.transform;
                projectileSpawnPointFollower.followPosition = false;
                projectileSpawnPointFollower.followRotation = true;
            }
        }

        public void SyncPlayerState()
        {
            if (_kinematicPlayerInstance == null)
            {
                return;
            }
        
            var inventory = _kinematicPlayerInstance.Inventory;
            var health = _kinematicPlayerInstance.Health;

            GlobalGameState.State.PlayerState.Health = health.currentHealth;
            GlobalGameState.State.PlayerState.Inventory = inventory.GetSerializedInventory();
            GlobalGameState.State.PlayerState.HeldItemIndex = inventory.ItemsInInventory.IndexOf(inventory.activelyHeldItem);
            GlobalGameState.State.PlayerState.EquippedItems = inventory.EquippedItems;
            SyncPlayerPosition();
        }

        private void SyncPlayerPosition()
        {
            if (_kinematicPlayerInstance == null)
            {
                return;
            }
        
            GlobalGameState.State.PlayerPosition = _kinematicPlayerInstance.Character.transform.position;
            GlobalGameState.State.PlayerRotation = _kinematicPlayerInstance.Character.transform.rotation.eulerAngles;
        }

        private void Update()
        {
            if (_kinematicPlayerInstance != null && !_kinematicPlayerInstance.Health.isDead) SyncPlayerState();
        }

        private void OnDestroy()
        {
            GameManager.Singleton.onPause.RemoveListener(HideUIOnPause);
            GameManager.Singleton.onResume.RemoveListener(ShowUIOnResume);
        }

        private void OnDrawGizmos()
        {
            if (defaultSpawnPoint == null)
            {
                Gizmos.color = new Color(1f, 0.67f, 0.05f);
                Gizmos.DrawSphere(transform.position, 0.8f);
            }
        }
    }
}