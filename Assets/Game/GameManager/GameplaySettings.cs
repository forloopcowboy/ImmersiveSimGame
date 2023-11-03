using System;
using System.Collections.Generic;
using Game.Src.EventBusModule;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameManager
{
    public class GameplaySettings : AbstractSettingsMenu
    {
        public List<SliderSetting> sliderSettings;

        private void Start()
        {
            LoadSettings();
            
            foreach (var setting in sliderSettings)
            {
                setting.Init();
            }
        }
        
        public override void SaveSettings()
        {
            foreach (var setting in sliderSettings)
            {
                PlayerPrefs.SetFloat(
                    setting.Key,
                    setting.Value
                );
            }
            
            LoadSettings();
        }

        public void LoadSettings()
        {
            foreach (var setting in sliderSettings)
            {
                setting.Slider.value = PlayerPrefs.GetFloat(
                    setting.Key,
                    setting.Value
                );
            }
            
            SceneEventBus.Emit(
                new GameplaySettingChanged {sliders=sliderSettings}
            );
        }
    }

    [Serializable]
    public class SliderSetting
    {
        public string Key;
        public Slider Slider;
        public float Value;

        public SliderSetting(string key)
        {
            Key = key;
        }

        public void Init()
        {
            Slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float arg0)
        {
            Value = arg0;
        }
    }

    public struct GameplaySettingChanged
    {
        public List<SliderSetting> sliders;
    }
}