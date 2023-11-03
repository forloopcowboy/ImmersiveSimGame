using System;
using UnityEngine;

namespace Game.DoorSystem
{
    [RequireComponent(typeof(Collider))]
    public class DoorLock : MonoBehaviour
    {
        /// <summary>
        /// Returns true if the door is cocked by overlapping
        /// triggers with the same tag as this object.
        /// </summary>
        public bool isCocked;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(gameObject.tag))
            {
                isCocked = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(gameObject.tag))
            {
                isCocked = false;
            }
        }
    }
}