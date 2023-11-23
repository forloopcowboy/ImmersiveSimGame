using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.HealthSystem;
using UnityEngine;

namespace Game.AIBehavior
{
    [TaskDescription("Returns success if damage was taken, otherwise returns OnFailure.")]
    public class HasTakenDamage : Conditional
    {
        public TaskStatus OnFailure = TaskStatus.Failure;
        public SharedGameObject damageSource;

        private bool _hasTakenDamage;
        private Health _health;

        public override void OnStart()
        {
            _health = GetComponent<Health>();
            _health.onDamage.AddListener(OnDamageTaken);
        }

        private void OnDamageTaken(GameObject arg0)
        {
            // ignore self damage
            if (gameObject.name != arg0.name)
            {
                _hasTakenDamage = true;
                damageSource.SetValue(arg0);
                Debug.Log($"ConditionalTask: {gameObject.name} Has taken damage from {arg0.name}!");
            }
            
            else Debug.Log($"ConditionalTask: [IGNORED SELF] {gameObject.name} has taken damage from {arg0.name}!");
        }
        
        public override TaskStatus OnUpdate()
        {
            return _hasTakenDamage ? TaskStatus.Success : OnFailure;
        }
        
        public override void OnEnd()
        {
            _health.onDamage.RemoveListener(OnDamageTaken);
            _hasTakenDamage = false;
        }
    }
}