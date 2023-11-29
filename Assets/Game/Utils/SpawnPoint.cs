using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Utils
{
    public class SpawnPoint : MonoBehaviour
    {
        [Required("Spawn ID is required to transition between scenes.")]
        public StringConstant SpawnID;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.5f);
            
            TextGizmo.Draw(transform.position + Vector3.up, gameObject.name);
        }
    }
}