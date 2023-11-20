using BehaviorDesigner.Runtime.Tasks;
using Game.DialogueSystem;

namespace Game.AIBehavior
{
    [TaskDescription("Returns success if dialogue has ended, otherwise returns OnFailure.")]
    public class HasDialogueEnded : Conditional
    {
        public TaskStatus OnFailure = TaskStatus.Failure;
        private bool _hasDialogueEnded;
    
        public override void OnStart()
        {
            Owner.RegisterEvent(DialogueInteractor.BehaviorEvents.DialogueEnded.ToString(), OnDialogueEnded);
        }

        public override TaskStatus OnUpdate()
        {
            return _hasDialogueEnded ? TaskStatus.Success : OnFailure;
        }

        private void OnDialogueEnded()
        {
            _hasDialogueEnded = true;
        }
    

        public override void OnEnd()
        {
            OnReset();
        }

        public override void OnReset()
        {
            Owner.UnregisterEvent(DialogueInteractor.BehaviorEvents.DialogueEnded.ToString(), OnDialogueEnded);
            _hasDialogueEnded = false;
        }
    }
}