using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RuinsRaiders.AI
{
    [CreateAssetMenu(fileName = "MultiAction", menuName = "RuinsRaiders/Ai/MultiAction", order = 1)]
    public class MultiAction : BasicAiActionData
    {
        [SerializeField]
        private bool parallel = false;

        [SerializeField]
        private List<BasicAiActionData> actions = new();

        public override BasicAiAction Create(ActivatorData trigger)
        {
            return new Action(trigger, this);
        }
        public class Action : BasicAiAction
        {
            readonly private MultiAction _parent;

            readonly private List<BasicAiAction> _actions;
            private int _currentAction = -1;

            public override bool CanStop()
            {
                if (_parent.parallel)
                {
                    foreach (var action in _actions)
                    {
                        if (!action.CanStop())
                            return false;
                    }
                    return true;
                }
                else
                {
                    if (_currentAction == -1)
                        return true;
                    else
                        return _actions.ElementAt(_currentAction).CanStop();
                }
            }

            public Action(ActivatorData trigger, MultiAction parent)
            {
                if (!parent.parallel && parent.actions.Count() > 0)
                    _currentAction = 0;

                this._actions = parent.actions.Select(it => it.Create(trigger)).ToList();
                this._parent = parent;
            }

            public override bool IsFinished()
            {
                if (_parent.parallel)
                {
                    foreach (var action in _actions)
                    {
                        if (!action.IsFinished())
                            return false;
                    }
                    return true;
                }
                else
                {
                    return _currentAction == -1;
                }
            }

            public override void Stop()
            {
                base.Stop();
                if (_parent.parallel)
                {
                    foreach (var action in _actions)
                        action.Stop();
                }
                else
                {
                    if (_currentAction == -1)
                        return;
                    else
                        _actions.ElementAt(_currentAction).Stop();
                }
            }

            public override void Update(float dt)
            {
                base.Stop();
                if (_parent.parallel)
                {
                    foreach (var action in _actions)
                        action.Update(dt);
                }
                else
                {
                    if (_currentAction == -1)
                        return;

                    if (_actions.ElementAt(_currentAction).IsFinished())
                    {
                        if (_currentAction >= _actions.Count() - 1)
                        {
                            _currentAction = -1;
                            return;
                        }
                        else
                            _currentAction++;
                    }

                    _actions.ElementAt(_currentAction).Update(dt);
                }
            }

            public override string ToString()
            {
                return "MultiAction;" +
                    "\n[" + _currentAction + "] " + ((_currentAction == -1) ? "none" :  _actions.ElementAt(_currentAction).ToString());
            }
        }
    }
}