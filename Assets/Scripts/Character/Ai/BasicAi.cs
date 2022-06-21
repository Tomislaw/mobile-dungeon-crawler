﻿using Assets.Scripts.Character.Ai;
using Assets.Scripts.Character.Ai.BasicAiState;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class BasicAi: MonoBehaviour
{
    public BasicAiStateData InitialState;
    internal BasicAiState state;

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
            state.Stop();
            state = transition.Value.Item1.State.Trigger(transition.Value.Item2);
            return;
        }
    }

    private void OnDisable()
    {
        if (state != null)
            state.Stop();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BasicAi))]
public class BasicAiEditor : Editor
{
    BasicAi ai;
    void OnEnable()
    {
        ai = target as BasicAi;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(ai.state != null)
        {
            EditorGUILayout.LabelField(ai.state.Name);
            if(ai.state.currentAction!=null)
                EditorGUILayout.TextArea(ai.state.currentAction.ToString());
        }
            
    }
}
#endif
