using Assets.Scripts.Character.Ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ShowDialogAction", menuName = "RuinsRaiders/Ai/ShowDialogAction", order = 1)]
public class ShowDialogAction : BasicAiActionData
{
    public GameObject dialog;
    public Vector2 offset;

    public float dialogTime = 0.3f;

    public override BasicAiAction Create(ActivatorData trigger)
    {
        return new Action(this, trigger);
    }

    public class Action : BasicAiAction
    {
        internal float dialogTime = 0;
        internal GameObject dialog;
        internal ShowDialogAction parent;

        public Action(ShowDialogAction parent, ActivatorData trigger)
        {
            if (trigger.triggeredFor == null)
                return;

            this.dialog = Instantiate(parent.dialog);
            dialog.transform.SetParent(trigger.triggeredFor.transform);
            dialog.transform.localPosition = parent.offset;
            this.dialogTime = parent.dialogTime;
            this.parent = parent;
        }

        public override bool CanStop()
        {
            return true;
        }
        public override bool IsFinished()
        {
            return dialog == null;
        }

        public override void Stop()
        {
            dialogTime = 0;
            if (dialog != null)
                Destroy(dialog);
        }

        public override void Update(float dt)
        {
            if (dialogTime <= 0)
            {
                if (dialog != null)
                    Destroy(dialog);
                return;
            }
            dialogTime -= dt;
        }
    }

}