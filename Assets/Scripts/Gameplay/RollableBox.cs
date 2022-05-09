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
    public bool CanRollLeft { 
        get => !IsFalling && !IsRolling
            && astar?.GetNode(astar.GetTileId(transform.position) + new Vector2Int(-1, 0)).Block == false; }
    public bool CanRollRight
    {
        get => !IsFalling && !IsRolling
            && astar?.GetNode(astar.GetTileId(transform.position) + new Vector2Int(1, 0)).Block == false;
    }
    public bool IsFalling { get; private set; }

    private Coroutine coroutine;

    private void Start()
    {
        astar = FindObjectOfType<AStar>();
        rotation = transform.rotation.eulerAngles.z;

        if (astar == null)
            return;
        astar.OnMapUpdated.AddListener(UpdateStandings);
        astar.AddDynamicBlockTile(astar.GetTileId(transform.position));
    }


    private void OnEnable()
    {
        if (astar == null)
            return;
        astar.AddDynamicBlockTile(astar.GetTileId(transform.position));
        UpdateStandings();
    }

    private void OnDisable()
    {
        if (astar == null)
            return;
        astar.RemoveDynamicBlockTile(astar.GetTileId(transform.position));
        UpdateStandings();

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
        if (pusher.transform.position.y > transform.position.y - 0.48)
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
            if (pusher.FaceLeft && !CanRollLeft)
                return;
            if (!pusher.FaceLeft && !CanRollRight)
                return;

             coroutine = StartCoroutine(Roll(pusher.FaceLeft));
        }
            

    }
    public void UpdateStandings()
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
    }

    private IEnumerator Fall()
    {
       if (IsFalling == true || IsRolling == true)
            yield break;

        IsFalling = true;

        if(isActiveAndEnabled)
            astar.RemoveDynamicBlockTile(astar.GetTileId(transform.position));

        int falledTiles = 0;
        var movement = 1f / FallPoints;

        var acceleration = 1f;
        var accelearationFactor = 1f - AccelerationPercentage;

        while (IsFalling)
        {

            for (int i = 0; i < FallPoints; i++)
            {
                transform.position -= new Vector3(0, movement, 0);
                yield return new WaitForSeconds((TimeToFall / (float)FallPoints) * acceleration);
            }

            // stop if reached block or platform
            var id = astar.GetTileId(transform.position);
            var bottomTile = astar.GetNode(id + new Vector2Int(0, -1));
            if (bottomTile.Block || bottomTile.Platform)
            {
                IsFalling = false;
                break;
            }

            acceleration *= accelearationFactor;
            falledTiles++;
        }

     
        if (astar == null)
        {
            coroutine = null;
            yield break;
        }

        if (isActiveAndEnabled)
            astar.AddDynamicBlockTile(astar.GetTileId(transform.position));

        yield return new WaitForSeconds((TimeToFall / (float)FallPoints));

        coroutine = null;
        UpdateStandings();
    }
    private IEnumerator Roll(bool left)
    {
        if (!isActiveAndEnabled)
            yield break;

        if (astar == null)
            yield break;

        if (IsFalling == true || IsRolling == true)
            yield break;

        IsRolling = true;
        IsFalling = false;

        astar.RemoveDynamicBlockTile(astar.GetTileId(transform.position));


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

        if (astar == null)
            yield break;
        astar.AddDynamicBlockTile(astar.GetTileId(transform.position));
    }

    public void ResetRotation()
    {
        rotation = 0;
        transform.transform.eulerAngles = new Vector3(0, 0, rotation);
    }

}

