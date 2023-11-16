using Game.DialogueSystem;
using Game.DoorSystem;
using Game.EquipmentSystem;
using Game.InteractionSystem;
using Game.Src.EventBusModule;
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
            
            InventoryContentUIController = FindObjectOfType<InventoryContentUIController>();
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
            HandleInteractionInput();

            if (!_isInDialogue)
            {
                HandleCharacterInput();
                HandleMouseInput();
            }
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Inventory.UseItemInHand();
            }
        }

        private void HandleInteractionInput()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                InventoryContentUIController.ToggleInventory();
                
                Debug.Log($"Inventory is now {(IsInventoryOpen ? "open" : "closed")}");
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!IsInventoryOpen && Inventory.TryToPickUpItem(out var item))
                {
                    SceneEventBus.Emit(new NotificationEvent($"Picked up {item.itemName}"));
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
                        var key = Inventory.ItemsInInventory.Find(item => item.Item is KeyItemType key && doorController.CanUnlock(key.password));
                        
                        if (key != null && key.Item is KeyItemType password)
                        {
                            SceneEventBus.Emit(new NotificationEvent($"Unlocking {doorController.itemName}..."));
                            doorController.Unlock(password);
                            doorController.PushDoor(Character.transform);
                        }
                        else
                        {
                            SceneEventBus.Emit(new NotificationEvent($"{doorController.itemName} is locked."));
                        }
                    }
                    else
                    {
                        doorController.PushDoor(Character.transform);
                    }
                }
                else if (Interactor.TryPeekInteractionQueue(out DialogueInteractor dialogueInteractor))
                {
                    if (dialogueInteractor.enabled)
                        Interactor.TryToInteract(out dialogueInteractor, "");
                    else SceneEventBus.Emit(new NextDialogueEvent()); // if interactor is no longer enabled, emit next dialogue event
                }
                else
                {
                    Debug.Log("Pressed E but inventory is closed. And no item is selected and no interactable in range. Emitting next dialog event in case there is one.");
                    SceneEventBus.Emit(new NextDialogueEvent());
                }
            }
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
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
    }
}