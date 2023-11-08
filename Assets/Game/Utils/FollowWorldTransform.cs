using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Utils
{
    public class FollowWorldTransform : SerializedMonoBehaviour
    {
        public Transform target;
        
        public bool followPosition;
        public bool followRotation;
        public bool followScale;
        
        [ShowIf("followPosition")] public float positionLerpSpeed = 0f;
        [ShowIf("followRotation")] public float rotationLerpSpeed = 0f;
        [ShowIf("followScale")] public float scaleLerpSpeed = 0f;
        
        private void Update()
        {
            if (followPosition && positionLerpSpeed > 0f)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, positionLerpSpeed * Time.deltaTime);
            } else if (followPosition)
            {
                transform.position = target.position;
            }

            if (followRotation && rotationLerpSpeed > 0f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotationLerpSpeed * Time.deltaTime);
            } else if (followRotation)
            {
                transform.rotation = target.rotation;
            }

            if (followScale && scaleLerpSpeed > 0f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, target.localScale, scaleLerpSpeed * Time.deltaTime);
            } else if (followScale)
            {
                transform.localScale = target.localScale;
            }
        }

        private void OnDrawGizmos()
        {
            // draw red line in the relative up direction of this transform for 10 units
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 10f);
            
            // draw blue line forward for 10 units
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10f);
        }
    }
}