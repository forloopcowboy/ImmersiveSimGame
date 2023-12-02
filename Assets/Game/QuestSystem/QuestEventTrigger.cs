using System;
using System.Collections.Generic;
using Game.Src.EventBusModule;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.QuestSystem
{
    /// <summary>
    /// Use this to trigger QuestEvents.
    /// </summary>
    [Serializable]
    public class QuestEventTrigger
    {
        public bool isEnabled;
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The event to trigger."), TabGroup("Quest")]
        public QuestEvent questEvent;
        public QuestCondition questCondition = new QuestCondition();
        
        public QuestEventTrigger(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }

        public bool CanTrigger => isEnabled && questCondition.CanTrigger;

        [Button, InfoBox("Triggers event if meets criteria above."), TabGroup("Quest")]
        public void Trigger()
        {
            if (CanTrigger) SceneEventBus.Emit(questEvent);
        }
    }
}