using System;
using System.Collections.Generic;
using Game.Src.EventBusModule;
using Game.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.InteractionSystem
{
    public class NotificationUIController : SerializedMonoBehaviour
    {
        public RectTransform notificationUIContainer;
        public GameObject notificationUIPrefab;

        private Action unsub;

        public void OnEnable()
        {
            unsub = SceneEventBus.Subscribe<NotificationEvent>(OnNotificationEvent);
            
            notificationUIContainer.gameObject.SetActive(false);
            
            // Delete all children
            foreach (Transform child in notificationUIContainer)
            {
                Destroy(child.gameObject);
            }
        }

        private void OnDisable()
        {
            unsub?.Invoke();
        }

        private void Update()
        {
            // If no more notifications to show, hide notification UI
            if (notificationUIContainer.childCount == 0)
            {
                notificationUIContainer.gameObject.SetActive(false);
            }
        }

        private void OnNotificationEvent(NotificationEvent obj)
        {
            // If notification UI is not active, activate it
            if (!notificationUIContainer.gameObject.activeSelf)
            {
                notificationUIContainer.gameObject.SetActive(true);
            }
            
            // Instantiate notification UI prefab
            var newNotification = Instantiate(notificationUIPrefab, notificationUIContainer);
            var text = newNotification.GetComponentInChildren<TMP_Text>();
            text.text = obj.message;
        }
    }
}