using TMPro;
using UnityEngine;

namespace Game.GameControls
{
    public class GameControlBindingUIController : MonoBehaviour
    {
        public TMP_Text BindingText;
        public TMP_Text NameText;
        
        public void SetBinding(GameControlBinding binding)
        {
            BindingText.text = binding.Binding;
            NameText.text = binding.Name;
        }
    }
}