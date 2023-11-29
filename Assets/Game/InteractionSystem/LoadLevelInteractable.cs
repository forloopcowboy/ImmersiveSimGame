using System;
using System.Collections;
using Game.SaveUtility;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace Game.InteractionSystem
{
    public class LoadLevelInteractable : InteractableObject
    {
        [ValueDropdown("GetSceneNames")]
        public int levelIndex;
        public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
        public StringConstant spawnPointId;

        private IEnumerable GetSceneNames()
        {
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                yield return new ValueDropdownItem(SceneManager.GetSceneByBuildIndex(i).name, i);
            }
        }
        
        public override void Interact(dynamic input)
        {
            GlobalGameState.SpawnState.RestoreLastPosition = String.IsNullOrEmpty(spawnPointId);
            GlobalGameState.SpawnState.SpawnID = spawnPointId;
            GameManager.GameManager.Singleton.LoadScene(levelIndex, loadSceneMode);
        }
    }
}