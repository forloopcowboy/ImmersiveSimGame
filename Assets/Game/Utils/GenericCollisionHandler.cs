using Game.Src.EventBusModule;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Utils
{
    /// <summary>
    /// Offers a generic implementation of ICollisionHandler.
    /// Simply uses Physics.IgnoreCollision to ignore collisions between all colliders in this object and the given object.
    /// </summary>
    public class GenericCollisionHandler : MonoBehaviour, ICollisionHandler
    {
        [InfoBox("Ignores collisions between all colliders in this object and the given object. Should be attached to projectile emitters to avoid self-damage on launch.")]
        
        public void IgnoreCollidersIn(GameObject obj)
        {
            SetCollisionHandling(gameObject, obj, true);
        }

        public void StopIgnoringCollidersIn(GameObject obj)
        {
            SetCollisionHandling(gameObject, obj, false);
        }

        public static void SetCollisionHandling(GameObject thisGameObject, GameObject otherGameObject, bool ignore)
        {
            var otherColliders = otherGameObject.GetComponentsInChildren<Collider>();
            var thisColliders = thisGameObject.GetComponentsInChildren<Collider>();

            if (ignore)
                Debug.Log($"[{thisGameObject.name}:Generic] Ignoring collisions with {otherGameObject.name}");
            else 
                Debug.Log($"[{thisGameObject.name}:Generic] Stopped ignoring collisions with {otherGameObject.name}");
            
            // this ignore others
            foreach (var pc in thisColliders)
            {
                foreach (var uc in otherColliders)
                {
                    Physics.IgnoreCollision(pc, uc, ignore);
                }
            }
            
            // others ignore this
            foreach (var uc in otherColliders)
            {
                foreach (var pc in thisColliders)
                {
                    Physics.IgnoreCollision(pc, uc, ignore);
                }
            }
        }
        
        public static void IgnoreCollisionsForSeconds(GameObject thisGameObject, GameObject otherGameObject, float seconds)
        {
            var collisionHandlers = thisGameObject.GetComponentsInChildren<ICollisionHandler>();
            
            var ignoreTime = seconds * Time.timeScale;
            
            // If no collision handlers are found, just ignore collisions for the given time using generic method
            if (collisionHandlers.Length == 0)
            {
                SetCollisionHandling(thisGameObject, otherGameObject, true);
                SceneEventBus.Singleton.StartCoroutine(
                    CoroutineHelpers.DelayedAction(
                        ignoreTime,
                        () => SetCollisionHandling(thisGameObject, otherGameObject, false)
                    )
                );
            }
            else
            {
                foreach (var collisionHandler in collisionHandlers)
                {
                    if (collisionHandler != null)
                    {
                        collisionHandler.IgnoreCollidersIn(otherGameObject);
                        SceneEventBus.Singleton.StartCoroutine(
                            CoroutineHelpers.DelayedAction(
                                ignoreTime,
                                () => collisionHandler.StopIgnoringCollidersIn(otherGameObject)
                            )
                        );
                    }
                }}
        }
    }
}