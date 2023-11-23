using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.ProjectileSystem
{
    public enum AimType
    {
        Raycast,
        Ballistic
    }
    
    public class AimComponent : SerializedMonoBehaviour
    {
        [InfoBox("Aim of type raycasts simply points the turret towards the target. Aim of type ballistic will calculate the ballistic trajectory of the projectile and point the turret towards the calculated launch direction.")]
        public AimType aimType = AimType.Raycast;
        
        [FormerlySerializedAs("cannonBase")] [FoldoutGroup("Turret")]
        public Transform traverse;

        [FormerlySerializedAs("cannonTurret")] [FoldoutGroup("Turret")]
        public Transform elevate;

        // "Movement speed in units per second."
        [FormerlySerializedAs("speed")] [FoldoutGroup("Aiming")]
        public float aimSpeed = 1.0F;

        // Time when the movement started.
        private float startTime;

        // Total distance between the markers.
        private float journeyLength;
        
        /// <summary>
        /// Attempts to get closest pawn and rotate towards it depending on the projectile launch type.
        /// </summary>
        public void AimAt(Vector3 target, float projectileSpeed, BallisticTrajectory projectileBallisticType = BallisticTrajectory.Min)
        {
            var projectileType = aimType;
            
            Debug.DrawLine(elevate.transform.position, target, Color.yellow, 0.1f);
                
            Vector3 relativePos = projectileType == AimType.Raycast ?
                target - elevate.position :
                ProjectileSpawner.CalculateBallisticVelocity(elevate.transform, target, projectileSpeed, projectileBallisticType);
            
            Debug.DrawRay(elevate.transform.position, relativePos, Color.blue, 0.2f);
            
            InterpolateTurretAim(relativePos);
        }

        /// <summary>
        /// Given a forward vector indicating the direction where the projectile
        /// is to be launched, interpolate the turret aim towards this direction.
        /// </summary>
        /// <param name="launchDirection">Direction of projectile emission</param>
        private void InterpolateTurretAim(Vector3 launchDirection)
        {
            Quaternion baseRot = Quaternion.LookRotation(launchDirection, Vector3.up);

            // first rotate completely towards target in world space
            traverse.rotation = Quaternion.Lerp(traverse.rotation, baseRot, Time.deltaTime * aimSpeed);
            elevate.rotation = Quaternion.Lerp(elevate.rotation, baseRot, Time.deltaTime * aimSpeed);

            // reset local roll & pitch for turret base
            traverse.localRotation = Quaternion.Euler(
                0,
                traverse.localRotation.eulerAngles.y, 
                0
            );
            // reset local roll & yaw for turret cannon
            elevate.localRotation = Quaternion.Euler(
                elevate.localRotation.eulerAngles.x, 
                0,
                0
            );
        }

        public void ResetAim()
        {
            if (traverse != null && traverse.parent != null)
            {
                InterpolateTurretAim(traverse.parent.forward);
            }
        }
    }

}