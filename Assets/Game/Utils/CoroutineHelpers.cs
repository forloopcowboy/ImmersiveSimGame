using System;
using System.Collections;
using UnityEngine;

namespace Game.Utils
{
    public static class CoroutineHelpers
    {
        public static IEnumerator DelayedAction(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }
    }
}