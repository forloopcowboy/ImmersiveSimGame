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
        public Transform spawnPoint;
        
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
        private DialoguePlayer _dialoguePlayerInstance;
        [ReadOnly]
        private DeathScreenUIController _deathScreenUIControllerInstance;
        [ReadOnly]
        private HealthBar _playerHealthBarInstance;

        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnPlayer();
            }
        }

        [Button]
        public void SpawnPlayer()
        {
            // TODO: pick spawn point from game state
            var position = spawnPoint != null ? spawnPoint.position : transform.position;
            var rotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
            
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
            
            // Check if player location should be loaded or initialized
            if (!GlobalGameState.State.PlayerLocationInitialized)
            {
                SyncPlayerPosition();
                Debug.Log("First time loading player location. Saving spawned position to serialized state.");
            }
            else
            {
                Debug.Log("Loading player location from serialized state.");
                _kinematicPlayerInstance.Character.Motor.enabled = false;
                _kinematicPlayerInstance.Character.transform.SetPositionAndRotation(GlobalGameState.State.PlayerPosition, Quaternion.Euler(GlobalGameState.State.PlayerRotation));
                _kinematicPlayerInstance.Character.Motor.SetPositionAndRotation(GlobalGameState.State.PlayerPosition, Quaternion.Euler(GlobalGameState.State.PlayerRotation));
            
                StartCoroutine(CoroutineHelpers.DelayedAction(0.5f, () =>
                {
                    _kinematicPlayerInstance.Character.Motor.enabled = true;
                }));
            }
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
                    // Reload scene
                    gameManager.ReloadScene();
                    // Load previous save
                    GlobalGameState.Singleton.LoadState();
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
            
            _dialoguePlayerInstance = gameUI.GetComponentInChildren<DialoguePlayer>();
            _deathScreenUIControllerInstance = deathScreenUI;
            _playerHealthBarInstance = playerHealthBar;

            // Handle game manager state
            gameManager.onPause.AddListener(HideUIOnPause);
            gameManager.onResume.AddListener(ShowUIOnResume);
        }

        private void HideUIOnPause()
        {
            _dialoguePlayerInstance.gameObject.SetActive(false);
            _deathScreenUIControllerInstance.gameObject.SetActive(false);
            _playerHealthBarInstance.Hide();
        }
        
        private void ShowUIOnResume()
        {
            _dialoguePlayerInstance.gameObject.SetActive(true);
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
            var inventory = _kinematicPlayerInstance.Inventory;
            var health = _kinematicPlayerInstance.Character.GetComponentInChildren<Health>();

            GlobalGameState.State.PlayerState.Health = health.currentHealth;
            GlobalGameState.State.PlayerState.Inventory = inventory.GetSerializedInventory();
            GlobalGameState.State.PlayerState.HeldItemIndex = inventory.ItemsInInventory.IndexOf(inventory.activelyHeldItem);
            GlobalGameState.State.PlayerState.EquippedItems = inventory.EquippedItems;
            SyncPlayerPosition();
        }

        private void SyncPlayerPosition()
        {
            GlobalGameState.State.PlayerPosition = _kinematicPlayerInstance.Character.transform.position;
            GlobalGameState.State.PlayerRotation = _kinematicPlayerInstance.Character.transform.rotation.eulerAngles;
        }

        private void OnDestroy()
        {
            GameManager.Singleton.onPause.RemoveListener(HideUIOnPause);
            GameManager.Singleton.onResume.RemoveListener(ShowUIOnResume);
        }

        private void OnDrawGizmos()
        {
            if (spawnPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(spawnPoint.position, 0.3f);
            }
            else
            {
                Gizmos.color = new Color(1f, 0.67f, 0.05f);
                Gizmos.DrawSphere(transform.position, 0.1f);
            }
        }
    }
}