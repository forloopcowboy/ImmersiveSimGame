using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue Preset", menuName = "Dialogue/New Preset", order = 0)]
    public class DialoguePreset : ScriptableObject
    {
        [InfoBox("Defines a list of dialogues and their conditions.")]
        public List<Dialogue> Dialogues = new ();
        
        public IEnumerable<Dialogue> GetPlayableDialogues()
        {
            foreach (var dialogue in Dialogues)
            {
                Debug.Log($"Checking if {dialogue.DialogueId} can play...");
                if (dialogue.CanPlay)
                {
                    Debug.Log($"Yes, {dialogue.DialogueId} can play.");
                    yield return dialogue;
                }
                else
                {
                    Debug.Log($"No, {dialogue.DialogueId} cannot play.");
                }
            }
        }
    }
}