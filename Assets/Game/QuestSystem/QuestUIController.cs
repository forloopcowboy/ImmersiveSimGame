using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.QuestSystem
{
    public class QuestUIController : SerializedMonoBehaviour
    {
        public Transform root;
        public GameObject questUIPrefab;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var newActive = !root.gameObject.activeSelf;
                root.gameObject.SetActive(newActive);
                if (newActive) UpdateQuestUI();
            }
        }

        private void UpdateQuestUI()
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Destroy(root.GetChild(i).gameObject);
            }
            
            foreach (var quest in QuestManager.Singleton.ActiveQuests)
            {
                var questUiElement = Instantiate(questUIPrefab, root);
                var text = questUiElement.GetComponentInChildren<TMP_Text>();
                if (quest.IsCompleted)
                    text.text = "(Completed) ";
                else text.text = "Goal: ";
                text.text += quest.IsCompleted ? quest.questName : quest.questEvents.Count == 0 ? quest.questName : quest.questEvents[^1].eventName;
            }
        }
    }
}