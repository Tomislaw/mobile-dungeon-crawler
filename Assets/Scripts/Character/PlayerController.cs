using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class PlayerController : MonoBehaviour
{
    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    private void Update()
    {
        var find = character.GetComponent<PathfindingController>();
        if (find != null)
        {
            if (Input.GetMouseButtonDown(0))
                find.MoveTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
            

       Vector2 move = new Vector2();

        if (Input.GetKey(KeyCode.A))
            move.x -= 1;
        if (Input.GetKey(KeyCode.D))
            move.x += 1;
        if (Input.GetKey(KeyCode.W))
            move.y += 1;
        if (Input.GetKey(KeyCode.S))
            move.y -= 1;

        character.Move = move;

        character.ChargeAttack = Input.GetKey(KeyCode.Space);
        if (Input.GetKeyUp(KeyCode.Space))
            character.Attack();
    }
}