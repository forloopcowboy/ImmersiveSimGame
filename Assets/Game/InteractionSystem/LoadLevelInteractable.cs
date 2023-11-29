using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.InteractionSystem
{
    public class LoadLevelInteractable : InteractableObject
    {
        [ValueDropdown("GetSceneNames")]
        public int levelIndex;
        public LoadSceneMode loadSceneMode = LoadSceneMode.Single;

        private IEnumerable GetSceneNames()
        {
            for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                yield return new ValueDropdownItem(UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(i).name, i);
            }
        }
        
        public override void Interact(dynamic input)
        {
            GameManager.GameManager.Singleton.LoadScene(levelIndex, loadSceneMode);
        }
    }
}