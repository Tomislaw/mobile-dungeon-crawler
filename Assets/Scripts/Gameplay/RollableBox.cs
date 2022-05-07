using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(100)]
public class RollableBox : MonoBehaviour
{
    public float TimeToRoll = 0.25f;
    public float TimeToPush = 0.2f;
    public float TimeToFall = 0.2f;

    [Range(0f, 1f)]
    public float AccelerationPercentage = 0.2f;
    public int Rotations = 6;
    public int FallPoints = 3;

    private float timeLeftToPush = 0;

    private MovementController pusher;
    private AStar astar;

    private float rotation = 0;
    public bool IsRolling { get; private set; }
    public bool CanRollLeft { get; private set; }
    public bool CanRollRight { get; private set; }

    public bool IsFalling { get; private set; }

    private Coroutine coroutine;

    private void OnEnable()
    {
        astar = FindObjectOfType<AStar>();
        astar?.AddDynamicBlockTile(astar.GetTileId(transform.position));
        astar?.OnMapUpdated.AddListener(UpdateStandings);
        UpdateStandings();
    }

    private void OnDisable()
    {
        astar?.RemoveDynamicBlockTile(astar.GetTileId(transform.position));
        astar?.OnMapUpdated.RemoveListener(UpdateStandings);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        var pusher = collision.gameObject.GetComponent<MovementController>();
        if (pusher)
        {
            this.pusher = pusher;
            timeLeftToPush = TimeToPush;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (pusher?.gameObject == collision.gameObject)
        {
            pusher = null;
            timeLeftToPush = TimeToPush;
        }
    }

    private void FixedUpdate()
    {
        if (coroutine!=null)
            return;

        if (pusher == null)
            return;

        // cannot push when standing on it
        if (pusher.transform.position.y > transform.position.y + 0.48)
        {
            timeLeftToPush = TimeToPush;
            return;
        }

        // cannot push when not moving
        if (!pusher.IsMoving)
        {
            timeLeftToPush = TimeToPush;
            return;
        }

        timeLeftToPush -= Time.fixedDeltaTime;
        if (timeLeftToPush < 0)
        {
            if (!CanRollLeft && pusher.FaceLeft)
                return;
            if (!CanRollRight && !pusher.FaceLeft)
                return;

             coroutine = StartCoroutine(Roll(pusher.FaceLeft));
        }
            

    }
    private void UpdateStandings()
    {
        if (coroutine != null)
            return;

        transform.position = new Vector3(
            (float)Math.Round(transform.position.x * 2, MidpointRounding.AwayFromZero) / 2,
            (float)Math.Round(transform.position.y * 2, MidpointRounding.AwayFromZero) / 2,
            transform.position.z);

        if (astar == null)
            return;

        var id = astar.GetTileId(transform.position);
        var bottomTile = astar.GetNode(id + new Vector2Int(0, -1));
        if (!bottomTile.Block && !bottomTile.Platform)
        {
            coroutine = StartCoroutine(Fall());
            return;
        }

        IsFalling = false;
        var leftTile = astar.GetNode(id + new Vector2Int(-1, 0));
        var rightTile = astar.GetNode(id + new Vector2Int(1, 0));
        CanRollLeft = !leftTile.Block;
        CanRollRight = !rightTile.Block;
    }

    private IEnumerator Fall()
    {
       if (IsFalling == true || IsRolling == true)
            yield break;

        CanRollLeft = false;
        CanRollRight = false;
        IsFalling = true;

        astar?.RemoveDynamicBlockTile(astar.GetTileId(transform.position));

        int falledTiles = 0;
        var movement = 1f / FallPoints;

        var acceleration = 1f;
        var accelearationFactor = 1f - AccelerationPercentage;

        while (IsFalling)
        {
            for (int i = 0; i < FallPoints; i++)
            {
                transform.position -= new Vector3(0, movement, 0);
                yield return new WaitForSeconds((TimeToFall / (float)FallPoints)* acceleration);
            }
            acceleration *= accelearationFactor;

            falledTiles++;

            var id = astar.GetTileId(transform.position);
            var bottomTile = astar.GetNode(id + new Vector2Int(0, -1));
            if (bottomTile.Block || bottomTile.Platform)
                IsFalling = false;
        }

        coroutine = null;
        astar?.AddDynamicBlockTile(astar.GetTileId(transform.position));
    }
    private IEnumerator Roll(bool left)
    {
        if (IsFalling == true || IsRolling == true)
            yield break;

        IsRolling = true;
        IsFalling = false;

        astar?.RemoveDynamicBlockTile(astar.GetTileId(transform.position));


        var sign = left ? 1 : -1;
        var step = sign * 90f / (float)Rotations;
        var movement = -1 * sign * 1f / (float)Rotations;

        for (int i = 0; i < Rotations; i++)
        {
            rotation += step;
            transform.transform.eulerAngles = new Vector3(0, 0, rotation);
            transform.position += new Vector3(movement, 0, 0);
            yield return new WaitForSeconds(TimeToRoll / (float)Rotations);
        }

        if (Mathf.Abs(rotation) > 360)
            rotation = 0;

        IsRolling = false;

        coroutine = null;
        astar?.AddDynamicBlockTile(astar.GetTileId(transform.position));
    }

    public void ResetRotation()
    {
        rotation = 0;
        transform.transform.eulerAngles = new Vector3(0, 0, rotation);
    }

}

