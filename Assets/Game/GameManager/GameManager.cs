using System;
using System.Collections;
using System.Collections.Generic;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game.GameManager
{
    public class GameManager : SerializedMonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Singleton => _instance;

        public UnityEvent onPause;
        public UnityEvent onResume;
        public UnityEvent onLoadingFinished;
        
        private float _timeScaleBeforePause;

        private void Start()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);
            
            DontDestroyOnLoad(gameObject);
            
            if (pauseScreen) 
                pauseScreen.SetActive(false);
            if (loadingScreen)
                loadingScreen.SetActive(false);
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
            if (!_isPaused)
            {
                onPause?.Invoke();
                _isPaused = true;
                Cursor.lockState = CursorLockMode.None;
                TogglePauseScreen(true);
            
                _timeScaleBeforePause = Time.timeScale;
                Time.timeScale = 0;
            }
        }

        public void Resume()
        {
            if (_isPaused)
            {
                onResume?.Invoke();
            
                _isPaused = false;
                Cursor.lockState = CursorLockMode.Locked;
                TogglePauseScreen(false);
            
                Time.timeScale = _timeScaleBeforePause;
            }
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
            ToggleLoadingScreen(true);

            StartCoroutine(CoroutineHelpers.DelayedAction(.15f, () =>
            {
                var totalScenes = scenes.Length;
                List<AsyncOperation> loadingProcesses = new List<AsyncOperation>(totalScenes);

                foreach (var sceneIndex in scenes)
                    loadingProcesses.Add(SceneManager.LoadSceneAsync(sceneIndex, mode));

                StartCoroutine(LoadScenesCoroutine(loadingProcesses, totalScenes));
            }));
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

            yield return new WaitForSeconds(0.25f);
            
            ToggleLoadingScreen(false);
            onLoadingFinished?.Invoke();
        }
        
        [Button]
        private void ToggleLoadingScreen(bool value)
        {
            if (loadingScreen != null) loadingScreen.SetActive(value);
        }
        
        [Button]
        private void TogglePauseScreen(bool value)
        {
            if (pauseScreen != null)
            {
                pauseScreen.SetActive(value);
            }
        }
        
        public void QuitApplication() => Application.Quit();
    }
}