using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuinsRaiders.AI
{
    public class BasicAi : MonoBehaviour
    {
        public BasicAiStateData initialState;

        internal BasicAiState _state;

        private void FixedUpdate()
        {
            if (_state == null)
            {
                ActivatorData trigger;
                trigger.triggeredBy = gameObject;
                trigger.triggeredFor = gameObject;
                _state = initialState.Trigger(trigger);
            }

            _state.Update(Time.fixedDeltaTime);

            if (_state.IsFinished())
            {
                ActivatorData data;
                data.triggeredBy = gameObject;
                data.triggeredFor = gameObject;
                _state = _state.OnActionFinished.Trigger(data);
                return;
            }

            var transition = _state.TransitionOccured();
            if (transition.HasValue)
            {
                _state.Stop();
                _state = transition.Value.Item1.State.Trigger(transition.Value.Item2);
                return;
            }
        }

        private void OnDisable()
        {
            if (_state != null)
                _state.Stop();
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
            if (ai._state != null)
            {
                EditorGUILayout.LabelField(ai._state.GetType().Name);
                if (ai._state.currentAction != null)
                    EditorGUILayout.TextArea(ai._state.currentAction.ToString());
            }

        }
    }
#endif
}