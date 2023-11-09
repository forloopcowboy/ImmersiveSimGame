using System;
using Game.Utils;
using UnityEngine;

namespace Game.ProjectileSystem
{
    public class ProjectileSpawner
    {
        public static Vector3 CalculateVelocityToLaunchToCameraDirection(GameObject camera, Transform source,
            float speed, LayerMask layerMask, float raycastRange = 15f)
        {
            // aim from camera infinitely far a our target
            var ray = new Ray(camera.transform.position, camera.transform.forward);

            if (Physics.Raycast(ray, out var hit, raycastRange, layerMask, QueryTriggerInteraction.Ignore))
            {
                return GetVelocityToHitTarget(source, hit.point, speed).normalized * speed;
            }

            else
                return GetVelocityToHitTarget(source, (ray.origin + ray.direction * raycastRange), speed).normalized *
                       speed;
        }
        
        /// <summary>
        /// Returns the velocity needed to hit a target from a certain position with a certain speed.
        /// </summary>
        /// <returns>The 3D velocity.</returns>
        public static Vector3 GetVelocityToHitTarget(Transform source, Vector3 target, float speed)
        {
            speed = Mathf.Clamp(speed, 0, speed);
            Vector3 toTarget = target - source.position;

            // Set up the terms we need to solve the quadratic equations.
            float gSquared = Physics.gravity.sqrMagnitude;
            float b = speed * speed + UnityEngine.Vector3.Dot(toTarget, Physics.gravity);
            float discriminant = b * b - gSquared * toTarget.sqrMagnitude;

            // Check whether the target is reachable at max speed or less.
            if (discriminant < 0)
            {
                // Target is too far away to hit at this speed.
                // Abort, or fire at max speed in its general direction?
                return toTarget.normalized * speed;
            }

            float discRoot = Mathf.Sqrt(discriminant);

            // Highest shot with the given max speed:
            float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

            // Most direct shot with the given max speed:
            float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

            // Lowest-speed arc available:
            float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

            float T = T_min; // choose T_max, T_min, or some T in-between like T_lowEnergy

            // Convert from time-to-hit to a launch velocity:
            return toTarget / T - Physics.gravity * T / 2f;
        }
    }
}