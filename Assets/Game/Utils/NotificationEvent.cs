using System;

namespace Game.Utils
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