using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders.UI
{
    // Responsible for single button for Mobile controls
    public class ArrowsHandleButton : MonoBehaviour
    {
        public UnityEvent onPointerDown = new();
        public UnityEvent onPointerUp = new();

        protected bool _selected = false;
        internal void SetSelected(bool selected)
        {
            if (this._selected != selected)
            {
                if (selected)
                    onPointerDown.Invoke();
                else
                {
                    onPointerUp.Invoke();
                }
                this._selected = selected;
            }

        }
    }
}