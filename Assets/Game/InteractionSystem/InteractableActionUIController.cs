using System;
using System.Collections;
using Game.EquipmentSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.InteractionSystem
{
    public class InteractableActionUIController : MonoBehaviour
    {
        [InfoBox("Will show top of pick up queue of this Inventory on ActionUIController on this object (or assigned below)."), SerializeField, Required("Assign ActionUIController")]
        public Interactor Interactor;
        public ActionUIController ActionUIController;

        private void Start()
        {
            if (ActionUIController == null)
            {
                ActionUIController = GetComponent<ActionUIController>();
            }
        
            if (Interactor == null)
            {
                throw new NullReferenceException("Inventory is null. Initialize it in the inspector or assign it in code.");
            }
            
        }

        private void Update()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (Interactor.TryPeekInteractionQueue(out InteractableObject item))
            {
                ActionUIController.SetText(item.itemName, item.interactionText, item.shortcut);
                ActionUIController.Show();
            }
            else
            {
                ActionUIController.Hide();
            }
        }
    }
}