using System;
using System.Collections.Generic;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.GameControls
{
    /// <summary>
    /// Displays the game controls UI.
    /// List of bindings is cleared every frame, so this list
    /// should be updated every frame by components who are listening
    /// for inputs.
    /// </summary>
    public class GameControlsUIController : SingletonMonoBehaviour<GameControlsUIController>
    {
        [Required] public GameControlBindingUIController UIElementPrefab;
        [Required] public RectTransform root;
        
        /// <summary>
        /// List of active controls. Push to this every frame a given control is active.
        /// </summary>
        public HashSet<GameControlBinding> ActiveBindings = new HashSet<GameControlBinding>(GameControlBinding.Comparer);
        public List<GameControlBindingUIController> UIElements = new List<GameControlBindingUIController>();

        public static void Display(string bindingName, string binding)
        {
            Singleton.ActiveBindings.Add(new GameControlBinding(bindingName, binding));
        }
        
        private void Update()
        {
            UpdateUIElements();
            ActiveBindings.Clear();
        }

        public void UpdateUIElements()
        {
            UpdateUIElementCountToMatchBinding();
            
            GameControlBinding[] sortedBindings = new GameControlBinding[ActiveBindings.Count];
            ActiveBindings.CopyTo(sortedBindings);
            Array.Sort(sortedBindings, (binding1, binding2) => String.Compare(binding1.Binding, binding2.Binding, StringComparison.Ordinal));
            
            for (int i = 0; i < ActiveBindings.Count; i++)
            {
                GameControlBindingUIController uiElement = UIElements[i];
                GameControlBinding binding = sortedBindings[i];
                uiElement.SetBinding(binding);
            }
        }

        private void UpdateUIElementCountToMatchBinding()
        {
            if (UIElements.Count != ActiveBindings.Count)
            {
                // If less, add UI elements
                // If more, delete UI elements
                // If same, do nothing
                int difference = ActiveBindings.Count - UIElements.Count;
                if (difference > 0)
                {
                    for (int i = 0; i < difference; i++)
                    {
                        var uiElement = Instantiate(UIElementPrefab, root);
                        UIElements.Add(uiElement);
                    }
                }
                else if (difference < 0)
                {
                    for (int i = 0; i < -difference; i++)
                    {
                        var uiElement = UIElements[UIElements.Count - 1];
                        UIElements.RemoveAt(UIElements.Count - 1);
                        Destroy(uiElement.gameObject);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Bindings should be unique by Binding.
    /// </summary>
    public struct GameControlBinding
    {
        public string Name;
        public string Binding;

        public GameControlBinding(string name, string binding)
        {
            Name = name;
            Binding = binding;
        }

        public static IEqualityComparer<GameControlBinding> Comparer = new GameControlBindingEqualityComparer();
    }

    public class GameControlBindingEqualityComparer : IEqualityComparer<GameControlBinding>
    {
        public bool Equals(GameControlBinding x, GameControlBinding y)
        {
            return x.Binding == y.Binding;
        }

        public int GetHashCode(GameControlBinding obj)
        {
            return HashCode.Combine(obj.Binding);
        }
    }
}