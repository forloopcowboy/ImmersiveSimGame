using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game.GameManager
{
    public class GameManager : SerializedMonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Singleton => _instance;

        public UnityAction onPause;
        public UnityAction onResume;
        public UnityAction onLoadingFinished;

        private void Start()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);
            
            if (pauseScreen) pauseScreen.SetActive(false);
        }

        public static bool IsPaused => Singleton._isPaused;
        
        private bool _isPaused;

        public GameObject pauseScreen;
        public GameObject loadingScreen;

        public static void TogglePauseState()
        {
            if (IsPaused) Singleton.Resume(); else Singleton.Pause();
        }

        public void Pause()
        {
            if (_isPaused == false) onPause?.Invoke();
            
            _isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            if (pauseScreen != null)
            {
                pauseScreen.SetActive(true);
            }
            
            Time.timeScale = 0;
        }

        public void Resume()
        {
            if (_isPaused == true) onResume?.Invoke();
            
            _isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            pauseScreen.SetActive(false);
            
            Time.timeScale = 1;
        }

        public void ReloadScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            
            LoadScene(activeScene.buildIndex);
        }
        
        public void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            LoadScenes(mode, sceneIndex);
        }

        public void LoadScenes(LoadSceneMode mode, params int[] scenes)
        {
            loadingScreen.SetActive(true);
            
            var totalScenes = scenes.Length;
            List<AsyncOperation> loadingProcesses = new List<AsyncOperation>(totalScenes);

            foreach (var sceneIndex in scenes)
                loadingProcesses.Add(SceneManager.LoadSceneAsync(sceneIndex, mode));

            StartCoroutine(LoadScenesCoroutine(loadingProcesses, totalScenes));
        }

        private IEnumerator LoadScenesCoroutine(List<AsyncOperation> loadingProcesses, int totalScenes)
        {
            var checkInterval = new WaitForSeconds(0.2f);
            int scenesLoaded = 0;
            
            while (scenesLoaded == totalScenes)
            {
                foreach (var sceneLoading in loadingProcesses)
                {
                    if (sceneLoading.isDone)
                        scenesLoaded++;
                }

                yield return checkInterval;
            }
            
            loadingScreen.SetActive(false);
            onLoadingFinished?.Invoke();
        }
        
        public void QuitApplication() => Application.Quit();
    }
}