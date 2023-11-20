using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AIBehavior
{
    public class HandleCharacterDeath : Action
    {
        public SharedFloat deathForce;
        public SharedVector3 deathTorque;
        
        private NavMeshAgent _agent;
        private Animator _animator;
        private Rigidbody _rigidbody;

        public override void OnStart()
        {
            if (_agent == null)
            {
                _agent = GetComponent<NavMeshAgent>();
            }
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            if (_rigidbody == null)
            {
                _rigidbody = gameObject.GetOrElseAddComponent<Rigidbody>();
            }
            
            if (_agent) _agent.enabled = false;
            if (_animator) _animator.enabled = false;
            
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.AddRelativeForce(transform.forward * -1f * deathForce.Value, ForceMode.Impulse);
            _rigidbody.AddRelativeTorque(deathTorque.Value, ForceMode.Impulse);
        }
    }
}