using System;
using Game.Utils;
using UnityEngine;

namespace Game.QuestSystem
{
    [CreateAssetMenu(fileName = "QuestEventId", menuName = "Quests/QuestEventId", order = 2), Serializable]
    public class QuestEventId : StringConstant
    {
        private void OnValidate()
        {
            Value = name;
        }
    }
}