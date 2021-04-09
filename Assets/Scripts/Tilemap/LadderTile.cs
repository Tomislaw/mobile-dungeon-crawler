using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.gameObject.GetComponent<Character>();
        if (character && character.CanUseLadder)
        {
            character.ladders.Add(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var character = collision.gameObject.GetComponent<Character>();
        if (character && character.CanUseLadder)
        {
            character.ladders.Remove(this);
        }
    }
}