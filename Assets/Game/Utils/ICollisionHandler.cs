using UnityEngine;

namespace Game.Utils
{
    public interface ICollisionHandler
    {
        /// <summary>
        /// Ignores collisions with any of the colliders in the given object.
        /// </summary>
        void IgnoreCollidersIn(GameObject obj);
        
        /// <summary>
        /// Stops ignoring collisions with any of the colliders in the given object.
        /// </summary>
        void StopIgnoringCollidersIn(GameObject obj);
    }
}