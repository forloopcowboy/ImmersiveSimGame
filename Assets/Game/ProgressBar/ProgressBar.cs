using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ProgressBar
{
    [ExecuteAlways]
    public class ProgressBar : MonoBehaviour
    {
        public Image mask;

        public float current;
        public float max;

        public bool isFull => current >= max;
        public bool isEmpty => current <= 0;
        public float unitsLeft => max - current;
        
        public float fill => Mathf.Clamp(current, 0f, max) / max;

        private void Update()
        {
            UpdateFill();
        }
        
        protected void UpdateFill()
        {
            mask.fillAmount = fill;
        }

        public IEnumerable<Image> GetImages
        {
            get
            {
                var images = new List<Image>();
                if (TryGetComponent(out Image imageThis)) images.Add(imageThis);
                images.AddRange(
                    GetComponentsInChildren<Image>()
                );

                return images;
            }
        }

        public void Toggle(bool value)
        {
            foreach (var image in GetImages) image.enabled = value;
        }

        private void OnEnable()
        {
            Show();
        }
        
        private void OnDisable()
        {
            Hide();
        }

        public void Show() => Toggle(true);
        public void Hide() => Toggle(false);
    }
}