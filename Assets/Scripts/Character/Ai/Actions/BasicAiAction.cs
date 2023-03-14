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

}