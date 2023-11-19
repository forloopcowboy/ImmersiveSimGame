using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using Game.Utils;
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
        
        public void StartPrinting()
        {
            if (_printCoroutine == null)
                _printCoroutine = StartCoroutine(PrintEnqueuedIcons());
        }
        
        private IEnumerator PrintEnqueuedIcons()
        {
            var interval = new WaitForSeconds(printInterval);
            camera.enabled = true;
            
            while (toPrint.TryDequeue(out var tuple))
            {
                var prefab = tuple.Item1;
                var callback = tuple.Item2;
                
                var instance = Instantiate(prefab, root);
                instance.transform.position = root.position;
                instance.transform.rotation = root.rotation;
                instance.transform.localScale = root.localScale;
                instance.layer = LayerMask.NameToLayer("Stage");
                instance.transform.ForEachChild(child => child.gameObject.layer = LayerMask.NameToLayer("Stage"));

                yield return interval;
                camera.FrameObject(instance);
                yield return interval;
                camera.Render(); 
                callback(camera.targetTexture.toTexture2D());

                Destroy(instance);
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
    }
}