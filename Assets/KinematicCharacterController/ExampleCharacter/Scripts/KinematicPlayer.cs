using System;
using Game.DialogueSystem;
using Game.DoorSystem;
using Game.EquipmentSystem;
using Game.GameControls;
using Game.GameManager;
using Game.GrabSystem;
using Game.HealthSystem;
using Game.InteractionSystem;
using Game.SaveUtility;
using Game.Src.EventBusModule;
using Game.Utils;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace KinematicCharacterController.ExampleCharacter.Scripts
{
    public class KinematicPlayer : MonoBehaviour
    {
        public Examples.KinematicCharacterController Character;
        public KinematicCharacterCamera CharacterCamera;
        public GameItemInventory Inventory;
        public InventoryContentUIController InventoryContentUIController;
        public Interactor Interactor;
        public GrabNode GrabNode;
        public Health Health;
        
        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";
        
        // State
        private bool _isInDialogue;
        
        public bool IsInventoryOpen => InventoryContentUIController.root.gameObject.activeSelf;

        /// <summary>
        /// If UI is open, returns true and the selected inventory item.
        /// </summary>
        /// <returns></returns>
        public bool TryToGetSelectedItem(out GameItemInInventory selectedItem)
        {
            selectedItem = null;
            if (!IsInventoryOpen) return false;

            return InventoryContentUIController.TryGetSelectedItem(out selectedItem);
        }
        private void Start()
        {
            Interactor = Character.gameObject.GetComponent<Interactor>() ?? GetComponent<Interactor>();
            if (!Interactor) throw new MissingComponentException("Missing Interactor on Character or player.");
            
            Inventory = Character.gameObject.GetComponent<GameItemInventory>();
            if (!Inventory) throw new MissingComponentException("Missing GameItemInventory on Character.");
            
            Health = Character.gameObject.GetComponent<Health>();
            if (!Health) throw new MissingComponentException("Missing Health on Character.");
            
            if (InventoryContentUIController == null) InventoryContentUIController = FindObjectOfType<InventoryContentUIController>();
            if (!InventoryContentUIController) throw new MissingComponentException("Missing InventoryContentUIController in scene.");
            
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
            
            // Handle dialogue interaction
            SceneEventBus.Subscribe<DialogueEvent>(OnDialogueEvent);
            SceneEventBus.Subscribe<EndDialogueEvent>(OnEndDialogueEvent);
        }

        public void OnValidate()
        {
            if (Character != null)
            {
                Inventory = Character.GetComponentInChildren<GameItemInventory>();
                Interactor = Character.GetComponentInChildren<Interactor>();
            }
            
            if (InventoryContentUIController == null)
            {
                InventoryContentUIController = FindObjectOfType<InventoryContentUIController>();
            }
            
            if (GrabNode == null && Character != null)
            {
                GrabNode = Character.GetComponentInChildren<GrabNode>();
            }
            
            if (Health == null && Character != null)
            {
                Health = Character.GetComponentInChildren<Health>();
            }
        }

        private void OnEndDialogueEvent(EndDialogueEvent obj)
        {
            _isInDialogue = false;
        }

        private void OnDialogueEvent(DialogueEvent obj)
        {
            _isInDialogue = true;
        }

        private void Update()
        {
            SyncActiveControls();
            HandleInteractionInput();

            if (!_isInDialogue && !GameManager.IsPaused && !Health.isDead && !IsInventoryOpen)
            {
                HandleCharacterInput();
                HandleMouseInput();
            }
            else ResetCharacterInput();
        }
        
        private void LateUpdate()
        {
            if (!GameManager.IsPaused && !Health.isDead && !IsInventoryOpen)
            {
                UpdateCamera();
            }
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (GrabNode.isGrabbed)
                    GrabNode.Throw();
                else Inventory.UseItemInHand();
            }
        }

        private void HandleInteractionInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (IsInventoryOpen)
                    InventoryContentUIController.ToggleInventory();
                else GameManager.TogglePauseState();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                InventoryContentUIController.ToggleInventory();
                Debug.Log($"Inventory is now {(IsInventoryOpen ? "open" : "closed")}");
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!IsInventoryOpen && Inventory.TryToPickUpItem(out var item))
                {
                    var pickUpDialogue = item.ItemType.GetPickUpDialogue();
                    var pickUpNotification = item.ItemType.GetPickUpNotification();
                    
                    if (!String.IsNullOrEmpty(pickUpNotification))
                    {
                        SceneEventBus.Emit(new NotificationEvent(pickUpNotification));
                    }
            
                    if (!String.IsNullOrEmpty(pickUpDialogue))
                    {
                        SceneEventBus.Emit(new DialogueEvent(new DialogueItem("", pickUpDialogue)));
                    }
                }
                else if (IsInventoryOpen)
                {
                    // ignore
                }
                else if (Interactor.TryToInteract<DoorController, string>(out var doorController, "default"))
                {
                    if (doorController.isLocked)
                    {
                        Debug.Log("Door is locked. Looking for key...");
                        var key = Inventory.ItemsInInventory.Find(item => item.ItemType is KeyItemType key && doorController.CanUnlock(key.password));
                        
                        if (key != null && key.ItemType is KeyItemType password)
                        {
                            if (!String.IsNullOrEmpty(doorController.unlockedMessage)) 
                                SceneEventBus.Emit(new NotificationEvent(doorController.unlockedMessage.Replace("$itemName", doorController.itemName)));
                            doorController.Unlock(password);
                            doorController.PushDoor(Character.transform);
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(doorController.lockedMessage)) 
                                SceneEventBus.Emit(new NotificationEvent(doorController.lockedMessage.Replace("$itemName", doorController.itemName)));
                        }
                    }
                    else
                    {
                        doorController.PushDoor(Character.transform);
                    }
                }
                else if (Interactor.TryPeekInteractionQueue(out DialogueInteractable dialogueInteractor))
                {
                    if (dialogueInteractor.enabled)
                        Interactor.TryToInteract(out dialogueInteractor, "");
                    else SceneEventBus.Emit(new NextDialogueEvent()); // if interactor is no longer enabled, emit next dialogue event
                }
                else if (Interactor.TryToInteract(out InteractableObject unknownInteractable, 0))
                {
                    // interact with any other interactable type
                }
            }
            
            // Handle dialogue controls
            if (_isInDialogue)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SceneEventBus.Emit(new NextDialogueEvent());
                }
                else if (Input.GetKeyDown(KeyCode.Tab))
                {
                    SceneEventBus.Emit(new SkipDialogueEvent());
                }
            }

            if (!IsInventoryOpen && Input.GetKeyDown(KeyCode.F))
            {
                if (GrabNode.isGrabbed)
                    GrabNode.Release();
                else
                    Interactor.TryToInteract(out GrabbableObject grabbable, GrabNode);
            }
        }

        private void UpdateCamera()
        {
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection =
                    Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation *
                    CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection =
                    Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }
        
        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif
            if (InventoryContentUIController.IsOpen) scrollInput = 0f;

            // Apply inputs to the camera
            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            if (!InventoryContentUIController.IsOpen && Input.GetMouseButtonDown(1))
            {
                CharacterCamera.TargetDistance = CharacterCamera.TargetDistance == 0f ? CharacterCamera.DefaultDistance : 0f;
            }
            
            // Switch interaction method depending on camera mode
            if (CharacterCamera.TargetDistance == 0f)
                Interactor.InteractionMethod = InteractionMethod.RAYCAST;
            else
                Interactor.InteractionMethod = InteractionMethod.TRIGGER;
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);
            characterInputs.SprintDown = Input.GetKeyDown(KeyCode.LeftShift);
            characterInputs.SprintUp = Input.GetKeyUp(KeyCode.LeftShift);

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }

        private void ResetCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            Character.SetInputs(ref characterInputs);
        }
        
        /// <summary>
        /// Displays the currently active control bindings to UI based on player state.
        /// </summary>
        private void SyncActiveControls()
        {
            // Esc to pause, unless inventory is open
            if (IsInventoryOpen)
            {
                GameControlsUIController.Display("Close inventory", KeyCode.Escape.ToString());
                
                // E can either use or equip item
                if (Inventory.HighlightedItem != null && Inventory.HighlightedItem.ItemType is EquipableItemType)
                    GameControlsUIController.Display("Equip", KeyCode.E.ToString());
                else
                    GameControlsUIController.Display("Use", KeyCode.E.ToString());
            }
            else if (!_isInDialogue)
            {
                // Space to jump
                GameControlsUIController.Display("Jump", KeyCode.Space.ToString());
                
                // Shift to sprint
                GameControlsUIController.Display("Sprint", KeyCode.LeftShift.ToString());
                
                // If interactable is close, E to interact
                if (Interactor.TryPeekInteractionQueue(out InteractableObject interactable))
                {
                    if (interactable is DoorController doorController)
                    {
                        if (doorController.isLocked)
                            GameControlsUIController.Display("Unlock", KeyCode.E.ToString());
                        else
                            GameControlsUIController.Display("Open", KeyCode.E.ToString());
                    }
                    else if (interactable is GrabbableObject)
                    {
                        GameControlsUIController.Display("Grab", KeyCode.F.ToString());
                    }
                    else if (interactable is DialogueInteractable)
                    {
                        GameControlsUIController.Display("Talk", KeyCode.E.ToString());
                    }
                    else
                    {
                        GameControlsUIController.Display("Interact", KeyCode.E.ToString());
                    }
                }
                
                if (Inventory.ActivelyHeldItem != null && Inventory.ActivelyHeldItem.ItemType is UsableItemType)
                    GameControlsUIController.Display("Use item", "Left click");
            }
            else if (_isInDialogue)
            {
                GameControlsUIController.Display("Next", KeyCode.E.ToString());
                if (DialogueSystem.CanSkipDialogue())
                    GameControlsUIController.Display("Skip", KeyCode.Tab.ToString());
            }
        }
    }
}