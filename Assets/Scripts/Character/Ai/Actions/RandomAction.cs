using System.Collections.Generic;
using UnityEngine;


namespace RuinsRaiders.AI
{
    [CreateAssetMenu(fileName = "RandomAction", menuName = "RuinsRaiders/Ai/RandomAction", order = 1)]
    public class RandomAction : BasicAiActionData
    {
        [SerializeField]
        private float timeToNextAction = -1;

        [SerializeField]
        private List<BasicAiActionData> actions = new();

        public override BasicAiAction Create(ActivatorData trigger)
        {
            return new Action(trigger, this);
        }
        public class Action : BasicAiAction
        {
            private BasicAiAction _currentAction;
            private readonly System.Random _random = new();
            private ActivatorData _trigger;
            private readonly RandomAction _parent;

            private float _timeToNextAction = 0;
            public override bool CanStop()
            {
                return _currentAction.CanStop();
            }

            public Action(ActivatorData trigger, RandomAction parent)
            {
                int index = _random.Next(parent.actions.Count);
                _currentAction = parent.actions[index].Create(trigger);
                _parent = parent;
                _trigger = trigger;
            }

            public override bool IsFinished()
            {
                return false;
            }

            public override void Stop()
            {
                base.Stop();
                _currentAction.Stop();
            }

            public override void Update(float dt)
            {
                if (
                    ((_timeToNextAction < 0) && _parent.timeToNextAction >= 0)
                    || _currentAction.IsFinished()
                    )

                    _timeToNextAction = _parent.timeToNextAction;

                int index = _random.Next(_parent.actions.Count);
                _currentAction = _parent.actions[index].Create(_trigger);
            }


        }
    }
}