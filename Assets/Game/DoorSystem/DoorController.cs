using System;
using System.Collections;
using Game.InteractionSystem;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.DoorSystem
{
    [SelectionBase]
    public class DoorController : InteractableObject
    {
        [TabGroup("Door")] public Rigidbody door;
        [TabGroup("Door")] public DoorLock doorLock;
        [TabGroup("Door")] public bool startLocked;
        [TabGroup("General"), Tooltip("You may use $itemName to refer to door name.")] public string lockedMessage = "The door is locked.";
        [TabGroup("General"), Tooltip("You may use $itemName to refer to door name.")] public string unlockedMessage = "Door unlocked.";

        [SerializeField, TabGroup("Door")] private StringConstant doorKey;
        
        [ShowInInspector, TabGroup("Door")]
        public bool isLocked { get; private set; }

        [ShowInInspector, ReadOnly, TabGroup("Door")]
        private int _openCount = 0;
        
        private void Start()
        {
            if (!door) throw new MissingComponentException("Missing Rigidbody 'door' on DoorController. Assign this to the door board. Ensure it has a collider.");
            if (!doorLock) throw new MissingComponentException("Missing DoorLock on DoorController.");
            
            if (startLocked) Lock();
        }
        
        private void UpdateActionName()
        {
            if (isLocked && _openCount == 0)
            {
                interactionText = "to unlock";
            }
            else
            {
                interactionText = "to push open";
            }
        }

        protected override void OnInteract(Interactor interactor, dynamic input)
        {
            if (input is string key)
                Unlock(key);
            else
            {
                Debug.Log("Not a valid key.");
            }
        }

        public void PushDoor(Transform source, float multiplier = 0.175f)
        {
            // if facing same direction as door forward, push with positive multiplier
            // if facing opposite direction as door forward, push with negative multiplier
            if (Vector3.Dot(source.forward, door.transform.forward) > 0)
                door.AddRelativeTorque(door.transform.up * -multiplier, ForceMode.Impulse);
            else
                door.AddRelativeTorque(door.transform.up * multiplier, ForceMode.Impulse);
        }
        
        public void Lock()
        {
            Debug.Log("Locking door " + itemName);
            isLocked = true;
            door.isKinematic = true;

            if (door.TryGetComponent(out HingeJoint joint))
            {
                door.transform.localRotation = Quaternion.identity;
            }
        }
        
        /// <param name="key">Code to attempt to unlock with.</param>
        /// <returns>True if can unlock OR requires no key, false if cannot unlock</returns>
        public bool CanUnlock(string key)
        {
            if (!isLocked) return true;
            if (doorKey == null) return true;
            if (key == doorKey) return true;
            
            return false;
        }
        
        /// <summary>
        /// Returns true if the door is unlocked.
        /// </summary>
        /// <param name="key">Key needed to unlock it.</param>
        public bool Unlock(string key)
        {
            if (!CanUnlock(key))
            {
                Debug.Log("Tried to unlock door but key was wrong.");
                return false;
            }

            if (isLocked && doorKey != null)
            {
                StartCoroutine(WaitForDoorToClose());
                Debug.Log("WOW! Unlocking door " + itemName);
            } else Debug.Log("Door is already unlocked. Pushing it.");
            
            isLocked = false;
            door.isKinematic = false;
            _openCount++;
            
            UpdateActionName();

            return true;
        }
        
        private IEnumerator WaitForDoorToClose()
        {
            yield return new WaitForSeconds(1f);

            while (!doorLock.isCocked)
            {
                yield return null;
            }
            
            Lock();
        }
    }
    
    
}