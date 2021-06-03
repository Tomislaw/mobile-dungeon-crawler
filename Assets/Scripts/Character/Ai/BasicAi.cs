using Assets.Scripts.Character.Ai;
using Assets.Scripts.Character.Ai.BasicAiState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class BasicAi: MonoBehaviour
{
    public BasicAiStateData InitialState;

    private BasicAiState state;

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        if(state == null)
        {
            ActivatorData trigger;
            trigger.triggeredBy = gameObject;
            trigger.triggeredFor = gameObject;
            state = InitialState.Trigger(trigger);
        }

        state.Update(Time.fixedDeltaTime);

        if (state.IsFinished())
        {
            ActivatorData data;
            data.triggeredBy = gameObject;
            data.triggeredFor = gameObject;
            state = state.OnActionFinished.Trigger(data);
            return;
        }

        var transition = state.TransitionOccured();
        if (transition.HasValue)
        {
            state = transition.Value.Item1.State.Trigger(transition.Value.Item2);
            return;
        }
    }

}

