using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Utils;
using Game.Utils.EventBusModule;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.AIBehavior
{
    [TaskDescription("Returns success if an actor with specified tag enters the zone.")]
    public class ActorEnteredZone : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The zone to check for actors in. Ensure this has one or more colliders.")]
        public SharedGameObject Zone;
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Tags to filter for when checking for actors in zone. If empty, all actors will be checked.")]
        public List<string> Tags = new();
        public TaskStatus OnFailure = TaskStatus.Failure;

        public SharedGameObject LatestActorInZone;
        public SharedGameObjectList AllActorsInZone;
        
        private Collider[] allColliders;

        public override void OnStart()
        {
            if (Zone.Value == null)
            {
                Debug.LogWarning("Zone is null. If you want the AI to respond to invasive threats, Ensure you have set the zone to check for actors.");
                return;
            }

            allColliders = Zone.Value.GetComponentsInChildren<Collider>();
            
            // clean up previous info
            LatestActorInZone.Value = null;
            AllActorsInZone.Value.Clear();

            foreach (var collider in allColliders)
            {
                if (collider.isTrigger)
                {
                    var doOnTrigger = collider.gameObject.GetOrElseAddComponent<DoOnTrigger>();
                    doOnTrigger.onTriggerEnter.AddListener(HandleActorEnteredZone);
                    doOnTrigger.onTriggerExit.AddListener(HandleActorExitedZone);
                }
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (LatestActorInZone.Value == null)
            {
                return OnFailure;
            }

            Debug.Log($"Actor {LatestActorInZone.Value.name} entered {Zone.Value.name} zone!");

            return TaskStatus.Success;
        }

        private void HandleActorEnteredZone(Collider arg0)
        {
            if (Tags.Count == 0 || Tags.Contains(arg0.gameObject.tag) || Tags.Contains(arg0.gameObject.transform.root.gameObject.tag))
            {
                LatestActorInZone.Value = arg0.gameObject;
                AllActorsInZone.Value.Add(arg0.gameObject);
            }
        }

        private void HandleActorExitedZone(Collider arg0)
        {
            AllActorsInZone.Value.Remove(arg0.gameObject);
            if (AllActorsInZone.Value.Count == 0)
            {
                LatestActorInZone.Value = null;
            }
        }

        public override void OnEnd()
        {
            if (allColliders == null) return;
            foreach (var collider in allColliders)
            {
                if (collider.isTrigger)
                {
                    var doOnTrigger = collider.gameObject.GetComponent<DoOnTrigger>();
                    if (!doOnTrigger) continue;
                    
                    doOnTrigger.onTriggerEnter.RemoveListener(HandleActorEnteredZone);
                    doOnTrigger.onTriggerExit.RemoveListener(HandleActorExitedZone);
                }
            }
        }
    }
}