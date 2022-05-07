using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Character.Ai.BasicAiState
{
    [CreateAssetMenu(fileName = "AiState", menuName = "RuinsRaiders/Ai/AiState", order = 1)]
    public class BasicAiStateData : ScriptableObject
    {
        public string Name;

        public BasicAiActionData Action;
        public BasicAiStateData OnActionFinished;
        public List<Transition> StateTransitions;

        [System.Serializable]
        public struct Transition
        {
            public BasicAiStateData State;
            public List<BasicAiActivatorData> Activators;
        }

        public BasicAiState Trigger(ActivatorData trigger)
        {
            return new BasicAiState(this, trigger);
        }
    }

    public class BasicAiState
    {
        public string Name;
        public BasicAiAction currentAction;
        public List<Transition> Transitions;
        public BasicAiStateData OnActionFinished;

        public BasicAiState(BasicAiStateData state, ActivatorData trigger)
        {
            this.Name = state.Name;
            if (state.Action != null)
                currentAction = state.Action.Create(trigger);

            Transitions = state.StateTransitions.ConvertAll(it => Transition.Create(it, trigger.triggeredBy));
            OnActionFinished = state.OnActionFinished;
        }

        public (Transition, ActivatorData)? TransitionOccured()
        {
            if(!CanStop())
                return null;

            foreach(var transiton in Transitions)
            {
                var trigger = transiton.Triggered();
                if (trigger.HasValue)
                {
                    return (transiton, trigger.Value);
                }
            }
            return null;
        }

        public void Update(float dt)
        {
            if (currentAction != null)
                currentAction.Update(dt);
        }

        public void Stop()
        {
            currentAction?.Stop();
        }

        public bool CanStop()
        {
            if (currentAction == null)
                return true;
            return currentAction.CanStop();
        }
        public bool IsFinished()
        {
            if (currentAction == null)
                return false;
            return currentAction.IsFinished();
        }



        public struct Transition
        {
            public List<BasicAiActivator> Activators;
            public BasicAiStateData State;

            public static Transition Create(BasicAiStateData.Transition transition, GameObject target)
            {
                Transition _transition;
                _transition.State = transition.State;
                _transition.Activators = transition.Activators.ConvertAll(it => it.Create(target));
                return _transition;
            }

            public ActivatorData? Triggered()
            {
                foreach(var activator in Activators)
                {
                    var trigger = activator.Triggered();
                    if (trigger != null)
                        return trigger;
                }
                return null;
            }
        }
    }

}