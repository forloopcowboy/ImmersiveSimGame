using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.QuestSystem
{
    [CreateAssetMenu(fileName = "New Quest Event", menuName = "Quests/Quest Event Preset", order = 0)]
    public class QuestEventPreset : ScriptableObject
    {
        [LabelText("Quest Event")]
        public QuestEventTrigger questEventTrigger = new QuestEventTrigger(true); 
    }
}