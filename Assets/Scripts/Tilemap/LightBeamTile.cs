using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeamTile : MonoBehaviour
{
    private LevelEvents levelEvents;
    public LayerMask playerLayer;

    private void Start()
    {
        if (levelEvents == null)
            levelEvents = FindObjectOfType<LevelEvents>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (levelEvents == null)
            return;

        var character = collision.gameObject.GetComponent<Character>();
        if (playerLayer == (playerLayer | (1 << collision.gameObject.layer)))
        {
            levelEvents.LevelFinished();
            StartCoroutine(character.HideCoroutine());
        }
    }
}