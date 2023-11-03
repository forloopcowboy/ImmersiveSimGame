using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Events;

namespace Game.GameManager
{
    public abstract class AbstractSettingsMenu : SerializedMonoBehaviour
    {
        public UnityAction goToMainMenu;
        public UnityAction<string> goToSettingsMenu;
        
        public void GoToMainMenu()
        {
            goToMainMenu?.Invoke();
        }

        public void GoToSettingsMenu(string settingsMenuName)
        {
            goToSettingsMenu?.Invoke(settingsMenuName);
        }

        public abstract void SaveSettings();
    }
}