using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(100)]
public class ChestTile : MonoBehaviour
{
    private LevelEvents levelEvents;

    public AdventureData.ChestData.Type type;
    private Animator animator;

    public UnityEvent OnOpen;
    public bool IsOpen;

    private void Start()
    {
        if (levelEvents == null)
            levelEvents = FindObjectOfType<LevelEvents>();

        animator = GetComponent<Animator>();

        if (IsOpen)
        {
            animator.Play("Open");
            var collider = GetComponent<Collider2D>();
            collider.enabled = false;
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (levelEvents == null)
            return;

        var character = collision.gameObject.GetComponent<PlayerController>();
        if (character)
        {
            animator.Play("Open");
            OnOpen.Invoke();
            var collider = GetComponent<Collider2D>();
            collider.enabled = false;
            IsOpen = true;
        }
    }
}