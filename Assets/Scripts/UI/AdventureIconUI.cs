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

    public GameObject ShowOnSelect;

    private AdventureIconUI[] AllIcons;

    private void Start()
    {
        AllIcons = FindObjectsOfType<AdventureIconUI>(true);
        Invalidate();
    }

    public void SetSelected(bool selected)
    {
        if (ShowOnSelect)
            ShowOnSelect.SetActive(selected);

        if (Selected == selected)
            return;

        this.Selected = selected;
        if (selected)
            foreach (var icon in AllIcons)
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

}