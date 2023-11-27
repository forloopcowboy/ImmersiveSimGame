using System;
using Game.EquipmentSystem;
using Game.GrabSystem;
using Game.HealthSystem;
using Game.InteractionSystem;
using Game.ProgressBar;
using Game.Utils;
using KinematicCharacterController.Examples;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KinematicCharacterController.ExampleCharacter.Scripts
{
    public class PlayerSpawner : MonoBehaviour
    {
        public Transform spawnPoint;
        
        public GameObject playerPrefab;
        public GameObject playerCharacterPrefab;
        public GameObject playerCameraPrefab;
        public GameObject gameUIPrefab;
        
        public bool spawnOnStart = true;

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
            var position = spawnPoint != null ? spawnPoint.position : transform.position;
            var rotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
            
            var player = Instantiate(playerPrefab, position, rotation);
            var playerCharacter = Instantiate(playerCharacterPrefab, position, rotation);
            var playerCamera = Instantiate(playerCameraPrefab, position, rotation);

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
            
            InitializePlayer(kinematicPlayer, kinematicCharacter, kinematicCamera);
            InitializeUI(kinematicPlayer);
        }

        private void InitializeUI(KinematicPlayer kinematicPlayer)
        {
            var gameUI = Instantiate(gameUIPrefab);
            var health = kinematicPlayer.Character.GetComponentInChildren<Health>();
            
            // initialize player HUD
            var deathScreenUI = gameUI.GetComponentInChildren<DeathScreenUIController>();
            if (deathScreenUI != null)
                deathScreenUI.health = health;
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