using BehaviorDesigner.Runtime.Tasks;
using Game.DialogueSystem;

namespace Game.AIBehavior
{
    [TaskDescription("Returns success if dialogue has started, otherwise returns OnFailure.")]
    public class HasDialogueStarted : Conditional
    {
        public TaskStatus OnFailure = TaskStatus.Failure;
        private bool _hasDialogueStarted;

        public override void OnStart()
        {
            Owner.RegisterEvent(DialogueInteractor.BehaviorEvents.DialogueStarted.ToString(), OnDialogueStarted);
        }

        public override TaskStatus OnUpdate()
        {
            return _hasDialogueStarted ? TaskStatus.Success : OnFailure;
        }

        private void OnDialogueStarted()
        {
            _hasDialogueStarted = true;
        }
        

        public override void OnEnd()
        {
            OnReset();
        }

        public override void OnReset()
        {
            Owner.UnregisterEvent(DialogueInteractor.BehaviorEvents.DialogueStarted.ToString(), OnDialogueStarted);
            _hasDialogueStarted = false;
        }
    }
}