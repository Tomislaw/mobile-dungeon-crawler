using Assets.Scripts.Character.Ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiAction", menuName = "RuinsRaiders/Ai/MultiAction", order = 1)]
public class MultiAction : BasicAiActionData
{
    public bool parallel = false;
    public List<BasicAiActionData> actions = new List<BasicAiActionData>();

    public override BasicAiAction Create(ActivatorData trigger)
    {
        return new Action(trigger, this);
    }
    public class Action : BasicAiAction
    {

        private ActivatorData trigger;
        private MultiAction parent;

        private List<BasicAiAction> actions;
        private int currentAction = -1;

        public override bool CanStop()
        {
            if (parent.parallel)
            {
                foreach (var action in actions)
                {
                    if (!action.CanStop())
                        return false;
                }
                return true;
            }
            else
            {
                if (currentAction == -1)
                    return true;
                else
                    return actions.ElementAt(currentAction).CanStop();
            }
        }

        public Action(ActivatorData trigger, MultiAction parent)
        {
            if (!parent.parallel && parent.actions.Count() > 0)
                currentAction = 0;

            this.actions = parent.actions.Select(it => it.Create(trigger)).ToList();
            this.parent = parent;
        }

        public override bool IsFinished()
        {
            if (parent.parallel)
            {
                foreach (var action in actions)
                {
                    if (!action.IsFinished())
                        return false;
                }
                return true;
            }
            else
            {
                return currentAction == -1;
            }
        }

        public override void Stop()
        {
            base.Stop();
            if (parent.parallel)
            {
                foreach (var action in actions)
                    action.Stop();
            }
            else
            {
                if (currentAction == -1)
                    return;
                else
                    actions.ElementAt(currentAction).Stop();
            }
        }

        public override void Update(float dt)
        {
            base.Stop();
            if (parent.parallel)
            {
                foreach (var action in actions)
                    action.Update(dt);
            }
            else
            {
                if (currentAction == -1)
                    return;

                if (currentAction == -1)
                    return;

                if (actions.ElementAt(currentAction).IsFinished())
                {
                    if (currentAction >= actions.Count() - 1)
                    {
                        currentAction = -1;
                        return;
                    }
                    else
                        currentAction++;
                }

                actions.ElementAt(currentAction).Update(dt);
            }
        }
    }
}