using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders.AI
{
    [CreateAssetMenu(fileName = "AiState", menuName = "RuinsRaiders/Ai/AiState", order = 1)]
    public class BasicAiStateData : ScriptableObject
    {
        public BasicAiActionData action;
        public BasicAiStateData onActionFinished;
        public List<Transition> stateTransitions;

        [System.Serializable]
        public struct Transition
        {
            public BasicAiStateData state;
            public List<BasicAiActivatorData> activators;
        }

        public BasicAiState Trigger(ActivatorData trigger)
        {
            return new BasicAiState(this, trigger);
        }
    }

    public class BasicAiState
    {
        public BasicAiAction currentAction;
        public List<Transition> Transitions;
        public BasicAiStateData OnActionFinished;

        public BasicAiState(BasicAiStateData state, ActivatorData trigger)
        {
            if (state.action != null)
                currentAction = state.action.Create(trigger);

            Transitions = state.stateTransitions.ConvertAll(it => Transition.Create(it, trigger.triggeredBy));
            OnActionFinished = state.onActionFinished;
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
                _transition.State = transition.state;
                _transition.Activators = transition.activators.ConvertAll(it => it.Create(target));
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