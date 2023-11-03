using System;
using System.Collections;
using Game.InteractionSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.DoorSystem
{
    public class DoorController : InteractableObject
    {
        public Rigidbody door;
        public DoorLock doorLock;
        public bool startLocked;
        
        public Vector3 direction;
        public float multiplier = 1f;
        
        [SerializeField] private string doorKey = "default";
        
        [ShowInInspector]
        public bool isLocked { get; private set; }
        
        private void Start()
        {
            if (!door) throw new MissingComponentException("Missing Rigidbody 'door' on DoorController. Assign this to the door board. Ensure it has a collider.");
            if (!doorLock) throw new MissingComponentException("Missing DoorLock on DoorController.");
            
            if (startLocked) Lock();
        }

        public override void Interact(dynamic input)
        {
            if (input is string key)
                Unlock(key);
            else
            {
                Debug.Log("Not a valid key.");
            }
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
        
        /// <summary>
        /// Returns true if the door is unlocked.
        /// </summary>
        /// <param name="key">Key needed to unlock it.</param>
        public bool Unlock(string key)
        {
            if (isLocked && key != doorKey)
            {
                Debug.Log("Tried to unlock door but key was wrong.");
                return false;
            }

            if (isLocked)
            {
                StartCoroutine(WaitForDoorToClose());
                Debug.Log("WOW! Unlocking door " + itemName);
            } else Debug.Log("Door is already unlocked. Pushing it.");
            
            isLocked = false;
            door.isKinematic = false;

            door.AddRelativeTorque(door.transform.up * multiplier, ForceMode.Impulse);

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