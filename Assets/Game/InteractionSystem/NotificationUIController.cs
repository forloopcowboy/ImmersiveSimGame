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
        public TMP_Text notificationUI;
        public DateTime lastNotificationTime;
        public float notificationDuration = 5f;
        public float rollOutSpeed = 1f;
        public bool showNotification;
        public Vector2 outOfScreenOffset = new Vector3(300, 0, 0);

        [ReadOnly, ShowInInspector]
        private Vector3 initialPosition;
        [ReadOnly, ShowInInspector]
        private Vector3 outOfScreenPosition;

        private Action unsub;

        public void OnEnable()
        {
            unsub = SceneEventBus.Subscribe<NotificationEvent>(OnNotificationEvent);
            
            notificationUIContainer.gameObject.SetActive(false);
            initialPosition = notificationUIContainer.anchoredPosition;
            outOfScreenPosition = new Vector3(
                initialPosition.x + outOfScreenOffset.x,
                initialPosition.y + outOfScreenOffset.y, 
                  initialPosition.z
            );
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
                var position = Vector2.Lerp(
                    notificationUIContainer.anchoredPosition,
                    initialPosition,
                    Time.deltaTime * rollOutSpeed
                );
                notificationUIContainer.anchoredPosition = position;
            }
            else
            {
                notificationUIContainer.gameObject.SetActive(false);
                notificationUIContainer.anchoredPosition = outOfScreenPosition;
                return;
            }
            
            if (DateTime.Now - lastNotificationTime > TimeSpan.FromSeconds(notificationDuration))
            {
                if (Vector3.Distance(notificationUIContainer.anchoredPosition, outOfScreenPosition) < 0.1f)
                {
                    notificationUIContainer.gameObject.SetActive(false);
                    showNotification = false;
                }
                else
                {
                    var position = Vector2.Lerp(
                        notificationUIContainer.anchoredPosition,
                        outOfScreenPosition,
                        Time.deltaTime * rollOutSpeed
                    );
                    notificationUIContainer.anchoredPosition = position;
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