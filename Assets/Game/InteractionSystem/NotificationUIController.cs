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

        private Vector3 initialPosition;
        private Vector3 outOfScreenPosition;

        public void Start()
        {
            SceneEventBus.Subscribe<NotificationEvent>(OnNotificationEvent);
            
            notificationUIContainer.gameObject.SetActive(false);
            initialPosition = notificationUIContainer.position;
            outOfScreenPosition = new Vector3(initialPosition.x - 280, initialPosition.y, initialPosition.z);
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
                if (notificationUIContainer.position.y < -50)
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