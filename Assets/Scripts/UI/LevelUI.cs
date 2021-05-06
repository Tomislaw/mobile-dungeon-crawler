using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[ExecuteAlways]
public class LevelUI : MonoBehaviour
{
    public Sprite locked;
    public Sprite hovered;
    public Sprite normal;

    public TMPro.TMP_Text text;

    public bool IsLocked;

    public Image image;

    public Button button;

    public AdventureData data;
    public int level;

    public float startDelay = 0.5f;

    public void Start()
    {
        Invalidate();
    }

    public void SetHovered(bool hovered)
    {
        if (IsLocked)
        {
            if (image)
                image.sprite = locked;
            if (text)
                text.gameObject.SetActive(false);
            if (button)
                button.interactable = false;
        }
        else
        {
            if (image)
                image.sprite = hovered ? this.hovered : this.normal;
            if (text)
                text.gameObject.SetActive(true);
            if (button)
                button.interactable = true;
        }
    }

    private void OnValidate()
    {
        Invalidate();
    }

    public void Invalidate()
    {
        if (data == null || data.levels.Count <= level || level < 0)
            return;

        var levelData = data.levels[level];
        IsLocked = !levelData.enabled;

        if (text)
            text.text = (level + 1).ToString();

        SetHovered(false);
    }

    public void StartLevel()
    {
        EventManager.TriggerEvent("Level Start");
        StartCoroutine("DelayedStart");
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(startDelay);
        SceneManager.LoadScene(data.levels[level].scene);
    }
}