using UnityEngine;
using UnityEngine.Events;

public class ArrowsHandleButton : MonoBehaviour
{
    public UnityEvent OnPointerDown;
    public UnityEvent OnPointerUp;

    bool selected = false;
    internal void SetSelected(bool selected)
    {
        if (this.selected != selected)
        {
            if (selected)
                OnPointerDown.Invoke();
            else
            {
                OnPointerUp.Invoke();
            }
            this.selected = selected;
        }
      
    }
}