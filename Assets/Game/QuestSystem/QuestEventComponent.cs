using UnityEngine;

namespace Game.QuestSystem
{
    public enum TriggerMethod
    {
        Collision,
        Trigger,
        FunctionCall
    }
    
    public class QuestEventComponent : MonoBehaviour
    {
        public TriggerMethod triggerMethod;
        public QuestEventTrigger questEventTrigger = new QuestEventTrigger(true);
        
        private void OnCollisionEnter(Collision other)
        {
            if (triggerMethod == TriggerMethod.Collision)
            {
                questEventTrigger.Trigger();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (triggerMethod == TriggerMethod.Trigger)
            {
                questEventTrigger.Trigger();
            }
        }
        
        public void Trigger()
        {
            questEventTrigger.Trigger();
        }
    }
}