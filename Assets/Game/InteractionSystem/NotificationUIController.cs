using System;
using System.Collections.Generic;
using Game.Src.EventBusModule;
using TMPro;
using UnityEngine;

namespace Game.InteractionSystem
{
    public class NotificationUIController : MonoBehaviour
    {
        public RectTransform notificationUIContainer;
        public TMP_Text notificationUI;
        public DateTime lastNotificationTime;
        public float notificationDuration = 5f;
        public float rollOutSpeed = 1f;
        public bool showNotification;
        public Vector3 outOfScreenOffset = new Vector3(-200, 0, 0);

        private Vector3 initialPosition;
        private Vector3 outOfScreenPosition;

        private Action unsub;

        public void OnEnable()
        {
            unsub = SceneEventBus.Subscribe<NotificationEvent>(OnNotificationEvent);
            
            notificationUIContainer.gameObject.SetActive(false);
            initialPosition = notificationUIContainer.position;
            outOfScreenPosition = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z) + outOfScreenOffset;
        }

        private void OnDisable()
        {
            unsub?.Invoke();
        }

        private void Update()
        {
            if (showNotification)
            {
                notificationUIContainer.gameObject.SetActive(true);
            }
            else
            {
                notificationUIContainer.gameObject.SetActive(false);
                return;
            }
            
            if (DateTime.Now - lastNotificationTime > TimeSpan.FromSeconds(notificationDuration))
            {
                if (Vector3.Distance(notificationUIContainer.position, outOfScreenPosition) < 0.1f)
                {
                    notificationUIContainer.gameObject.SetActive(false);
                    notificationUIContainer.position = initialPosition;
                    showNotification = false;
                }
                else
                {
                    notificationUIContainer.position = Vector3.Lerp(notificationUIContainer.position, outOfScreenPosition, Time.deltaTime * rollOutSpeed);
                }
            }
        }

        private void OnNotificationEvent(NotificationEvent obj)
        {
            showNotification = true;
            lastNotificationTime = DateTime.Now;
            notificationUI.text = obj.message;
        }
    }
}