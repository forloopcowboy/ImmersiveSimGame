using System;
using System.Collections.Generic;
using Game.GameManager;
using TMPro;
using UnityEngine;

namespace Core.Scripts
{
    public class GraphicsSettings : AbstractSettingsMenu
    {
        public TMP_Dropdown resolutionDropdown;
        public TMP_Dropdown qualityDropdown;
        public TMP_Dropdown textureDropdown;
        public TMP_Dropdown aaDropdown;
        Resolution[] resolutions;

        private void Start()
        {
            
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            resolutions = Screen.resolutions;
            int currentResolutionIndex = 0;
            
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + 
                                resolutions[i].height;
                
                options.Add(option);
                
                if (resolutions[i].width == Screen.currentResolution.width 
                    && resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.RefreshShownValue();
            
            LoadSettings(currentResolutionIndex);
            
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }
        
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, 
                resolution.height, Screen.fullScreen);
        }
        
        public void SetTextureQuality(int textureIndex)
        {
            QualitySettings.globalTextureMipmapLimit = textureIndex;
            qualityDropdown.value = 3;
        }
        
        public void SetAntiAliasing(int aaIndex)
        {
            QualitySettings.antiAliasing = aaIndex;
            qualityDropdown.value = 3;
        }
        
        public void SetQuality(int qualityIndex)
        {
            if (qualityIndex != 3) // if the user is not using 
                //any of the presets
                QualitySettings.SetQualityLevel(qualityIndex);
            
            switch (qualityIndex)
            {
                case 0: // quality level - high
                    textureDropdown.value = 0;
                    aaDropdown.value = 0;
                    break;
                case 1: // quality level - medium
                    textureDropdown.value = 1;
                    aaDropdown.value = 0;
                    break;
                case 2: // quality level - low
                    textureDropdown.value = 2;
                    aaDropdown.value = 0;
                    break;
            }
            
            qualityDropdown.value = qualityIndex;
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }
        
        public override void SaveSettings()
        {
            PlayerPrefs.SetInt("QualitySettingPreference", 
                qualityDropdown.value);
            PlayerPrefs.SetInt("ResolutionPreference", 
                resolutionDropdown.value);
            PlayerPrefs.SetInt("TextureQualityPreference", 
                textureDropdown.value);
            PlayerPrefs.SetInt("AntiAliasingPreference", 
                aaDropdown.value);
            PlayerPrefs.SetInt("FullscreenPreference", 
                Convert.ToInt32(Screen.fullScreen));
        }
        
        public void LoadSettings(int currentResolutionIndex)
        {
            if (PlayerPrefs.HasKey("QualitySettingPreference"))
            {
                var qualityDropdownValue = PlayerPrefs.GetInt("QualitySettingPreference");
                
                qualityDropdown.options.Clear();
                foreach (var name in QualitySettings.names)
                    qualityDropdown.options.Add(new TMP_Dropdown.OptionData(name));

                qualityDropdown.value = qualityDropdownValue;
            }
            else
                qualityDropdown.value = 3;
            if (PlayerPrefs.HasKey("ResolutionPreference"))
                resolutionDropdown.value = 
                    PlayerPrefs.GetInt("ResolutionPreference");
            else
                resolutionDropdown.value = currentResolutionIndex;
            if (PlayerPrefs.HasKey("TextureQualityPreference"))
                textureDropdown.value = 
                    PlayerPrefs.GetInt("TextureQualityPreference");
            else
                textureDropdown.value = 0;
            if (PlayerPrefs.HasKey("AntiAliasingPreference"))
                aaDropdown.value = 
                    PlayerPrefs.GetInt("AntiAliasingPreference");
            else
                aaDropdown.value = 1;
            if (PlayerPrefs.HasKey("FullscreenPreference"))
                Screen.fullScreen = 
                    Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
            else
                Screen.fullScreen = true;
            
            // if (PlayerPrefs.HasKey("VolumePreference"))
            //     volumeSlider.value = 
            //         PlayerPrefs.GetFloat("VolumePreference");
            // else
            //     volumeSlider.value = 
            //         PlayerPrefs.GetFloat("VolumePreference");
        }

    }
}