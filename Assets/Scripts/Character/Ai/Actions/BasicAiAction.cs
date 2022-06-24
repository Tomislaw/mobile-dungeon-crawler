using UnityEngine;

namespace RuinsRaiders.AI
{
    public abstract class BasicAiAction
    {
        internal bool stopped;
        public abstract bool CanStop();

        public virtual void Stop()
        {
            stopped = true;
        }
        public abstract bool IsFinished();

        public abstract void Update(float dt);

    }


    public abstract class BasicAiActionData : ScriptableObject
    {
        public abstract BasicAiAction Create(ActivatorData trigger);
    }
    [CreateAssetMenu(fileName = "IdleAction", menuName = "RuinsRaiders/Ai/IdleAction", order = 1)]
    public class IdleAction : BasicAiActionData
    {
        public float idlingTime;
        public override BasicAiAction Create(ActivatorData trigger)
        {
            return new Action(this);
        }

        public class Action : BasicAiAction
        {
            private float _idlingTime;
            public override bool CanStop()
            {
                return true;
            }

            public Action(IdleAction parent)
            {
                _idlingTime = parent.idlingTime;
            }

            public override bool IsFinished()
            {
                return _idlingTime < 0;
            }

            public override void Update(float dt)
            {
                if (stopped)
                    return;
                _idlingTime -= dt;
            }
        }
    }
}