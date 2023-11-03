using TMPro;
using UnityEngine;

namespace Game.EquipmentSystem
{
    /// <summary>
    /// Controls text that says:
    /// [E] to pick up [Item name]
    /// </summary>
    public class ActionUIController : MonoBehaviour
    {
        public TMP_Text ActionText;
        public TMP_Text ItemNameText;
        public TMP_Text ShortcutText;
        
        public void Show()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        
        public void SetText(string itemName, string actionName = "to pick up", string shortcut = "E")
        {
            SetItemNameText(itemName);
            SetActionText(actionName);
            SetShortcutText(shortcut);
        }

        private void SetShortcutText(string shortcut)
        {
            ShortcutText.text = "[" + shortcut + "]";
        }

        private void SetActionText(string actionName)
        {
            ActionText.text = actionName;
        }

        private void SetItemNameText(string itemName)
        {
            ItemNameText.text = itemName;
        }
    }
}