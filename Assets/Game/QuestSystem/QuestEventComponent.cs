using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.QuestSystem
{
    public enum TriggerMethod
    {
        Collision,
        Trigger,
        FunctionCall
    }
    
    public class QuestEventComponent : MonoBehaviour
    {
        public TriggerMethod triggerMethod;
                
        [ShowInInspector]
        public bool IsUsingPreset
        {
            get => questEventPreset != null;
            set
            {
                if (value)
                {
                    if (questEventPreset == null)
                    {
                        questEventPreset = ScriptableObject.CreateInstance<QuestEventPreset>();
                        questEventPreset.name = "New Quest Event";
                        questEventPreset.questEventTrigger = questEventTrigger; // Auto initialize from current values to make it easier to transition to presets.

#if UNITY_EDITOR
                        try
                        {
                            var path = "Assets/Game/Quests/" + questEventPreset.name + ".asset";
                            AssetDatabase.CreateAsset(questEventPreset, path);
                            Selection.activeObject = questEventPreset;
                            
                            EditorUtility.SetDirty(this);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Failed to create asset.");
                            Console.WriteLine(e);
                        }
#endif
                    }
                    questEventTrigger = questEventPreset.questEventTrigger;
                }
            }
        }
        [SerializeField, InlineEditor(InlineEditorModes.FullEditor)]
        protected QuestEventPreset questEventPreset;
        [SerializeField, HideIf("IsUsingPreset")]
        protected QuestEventTrigger questEventTrigger = new QuestEventTrigger(true);

        public QuestEventTrigger QuestEventTrigger => questEventPreset != null ? questEventPreset.questEventTrigger : questEventTrigger;

        
        private void OnCollisionEnter(Collision other)
        {
            if (triggerMethod == TriggerMethod.Collision)
            {
                Trigger();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (triggerMethod == TriggerMethod.Trigger)
            {
                Trigger();
            }
        }
        
        public void Trigger()
        {
            QuestEventTrigger.Trigger();
        }
    }
}