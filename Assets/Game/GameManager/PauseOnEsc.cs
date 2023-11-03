using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.GameManager
{
    public class PauseOnEsc : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.TogglePauseState();
            }
        }
    }
}