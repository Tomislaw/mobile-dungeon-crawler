using Assets.Scripts.Character.Ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomAction", menuName = "RuinsRaiders/Ai/RandomAction", order = 1)]
public class RandomAction : BasicAiActionData
{
    public float timeToNextAction = -1;
    public List<BasicAiActionData> actions = new List<BasicAiActionData>();

    public override BasicAiAction Create(ActivatorData trigger)
    {
        return new Action(trigger, this);
    }
    public class Action : BasicAiAction
    {
        private BasicAiAction currentAction;
        private System.Random random = new System.Random();
        private ActivatorData trigger;
        private RandomAction parent;

        private float _timeToNextAction = 0;
        public override bool CanStop()
        {
            return currentAction.CanStop();
        }

        public Action(ActivatorData trigger, RandomAction parent)
        {
            int index = random.Next(parent.actions.Count);
            currentAction = parent.actions[index].Create(trigger);
            this.parent = parent;
        }

        public override bool IsFinished()
        {
            return false;
        }

        public override void Stop()
        {
            base.Stop();
            currentAction.Stop();
        }

        public override void Update(float dt)
        {
            if (
                ((_timeToNextAction < 0) && parent.timeToNextAction >= 0)
                || currentAction.IsFinished()
                )

                _timeToNextAction = parent.timeToNextAction;

            int index = random.Next(parent.actions.Count);
            currentAction = parent.actions[index].Create(trigger);
        }


    }
}