using System;

namespace Game.InteractionSystem
{
    [Serializable]
    public class NotificationEvent
    {
        public string message;
        
        public NotificationEvent(string message)
        {
            this.message = message;
        }
    }
}