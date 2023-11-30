using System.Collections.Generic;
using Game.EquipmentSystem;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.SaveUtility
{
    public class ItemDatabase : SingletonMonoBehaviour<ItemDatabase>
    {
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public List<GameItemType> GameItemAssets = new ();
        
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name); //FindAssets uses tags check documentation for more info
            
            T[] a = new T[guids.Length];
            
            for(int i =0; i<guids.Length; i++) //probably could get optimized
            {
                var guid = guids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }

        public static bool TryGetItem<TGameItemType>(string identifier, out TGameItemType foundGameItem) where TGameItemType : ScriptableObject, IHasIdentifier
        {
            foundGameItem = default;
            
            foreach (var gameItemAssets in Singleton.GameItemAssets)
            {
                if (gameItemAssets is TGameItemType gameItemType && gameItemType.Identifier == identifier)
                {
                    foundGameItem = gameItemType;
                    return true;
                }
            }

            return false;
        }
    }
}