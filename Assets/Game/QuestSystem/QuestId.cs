using System;
using Game.Utils;
using UnityEngine;

namespace Game.QuestSystem
{
    [CreateAssetMenu(fileName = "QuestId", menuName = "Quests/QuestID", order = 1), Serializable]
    public class QuestId : StringConstant
    {
        private void OnValidate()
        {
            Value = name;
        }
    }
}