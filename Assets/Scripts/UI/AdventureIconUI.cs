using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureIconUI : MonoBehaviour
{
    [SerializeField]
    private bool Selected;

    [SerializeField]
    [HideInInspector]
    private bool Hovered;

    public Sprite normalSprite;
    public Sprite hoveredSprite;

    public List<Image> images = new List<Image>();

    public void SetSelected(bool selected)
    {
        if (Selected == selected)
            return;

        this.Selected = selected;
        if (selected)
            foreach (var icon in transform.parent.GetComponentsInChildren<AdventureIconUI>())
            {
                if (icon.gameObject == gameObject)
                    continue;

                icon.SetSelected(false);
            }

        Invalidate();
    }

    public void Invalidate()
    {
        foreach (var image in images)
        {
            image.sprite = Selected || Hovered ? hoveredSprite : normalSprite;
        }
    }

    public void SetHovered(bool hovered)
    {
        Hovered = hovered;
        Invalidate();
    }

    private void Start()
    {
        Invalidate();
    }
}