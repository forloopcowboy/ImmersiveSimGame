using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Utils;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Src.IconGeneration
{
    public class IconPrinter : SingletonMonoBehaviour<IconPrinter>
    {
        public Camera camera;
        public Transform root;

        public float printInterval = 0.05f;

        private void Start()
        {
            if (!camera) camera = GetComponentInChildren<Camera>();
            if (!root) root = transform.GetChild(0);
        }
        
        private Queue<Tuple<GameObject, Action<Texture2D>>> toPrint = new ();

        public void GetIcon(GameObject prefab, Action<Texture2D> callback)
        {
            toPrint.Enqueue(new Tuple<GameObject, Action<Texture2D>>(prefab, callback));
            StartPrinting();
        }

        private Coroutine _printCoroutine = null;
        private EditorCoroutine _printEditorCoroutine = null;
        
        public void StartPrinting()
        {
#if UNITY_EDITOR
            if (_printEditorCoroutine == null && _printCoroutine != null)
                _printEditorCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(PrintEnqueuedIcons());
#endif
            if (_printCoroutine == null && _printEditorCoroutine == null)
                _printCoroutine = StartCoroutine(PrintEnqueuedIcons());
        }
        
        private IEnumerator PrintEnqueuedIcons()
        {
            Debug.Log("Starting icon printing...");
            
            var interval = new WaitForSeconds(printInterval);
            camera.enabled = true;
            
            while (toPrint.TryDequeue(out var tuple))
            {
                var prefab = tuple.Item1;
                var callback = tuple.Item2;
                Debug.Log($"Printing icon for {prefab.name}...");
                
                var instance = Instantiate(prefab, root);
                instance.transform.position = root.position;
                instance.transform.rotation = root.rotation;
                instance.transform.localScale = root.localScale;
                instance.layer = LayerMask.NameToLayer("Stage");
                instance.transform.ForEachChild(child => child.gameObject.layer = LayerMask.NameToLayer("Stage"));
                
                yield return interval;
                Debug.Log($"Framing {prefab.name}...");
                camera.FrameObject(instance);
                yield return interval;
                Debug.Log($"Rendering icon for {prefab.name}...");
                camera.Render();
                callback(camera.targetTexture.toTexture2D());

                Debug.Log($"Destroying temp instance of {prefab.name}...");
                if (Application.isPlaying)
                    Destroy(instance);
                else
                    DestroyImmediate(instance);
                
                yield return interval;
            }

            camera.enabled = false;
            _printCoroutine = null;
        }
        
        
    }
    
    public static class IconPrinterExtension
    {
        public static Texture2D toTexture2D(this RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA64, false);
            var old_rt = RenderTexture.active;
            RenderTexture.active = rTex;

            // TODO: ignore black pixels and render them transparent
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();

            RenderTexture.active = old_rt;
            return tex;
        }
        
        public static Sprite SaveSpriteToEditorPath (this Sprite sp, string path) {
 
            string dir = Path.GetDirectoryName (path);
 
            Directory.CreateDirectory (dir);
 
            File.WriteAllBytes(path, sp.texture.EncodeToPNG());
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
 
            TextureImporter ti = AssetImporter.GetAtPath (path) as TextureImporter;
 
            ti.spritePixelsPerUnit = sp.pixelsPerUnit;
            ti.mipmapEnabled = false;
            ti.textureType = TextureImporterType.Sprite;
            EditorUtility.SetDirty (ti);
            ti.SaveAndReimport ();
 
            return AssetDatabase.LoadAssetAtPath(path, typeof (Sprite)) as Sprite;
        }
    }
}