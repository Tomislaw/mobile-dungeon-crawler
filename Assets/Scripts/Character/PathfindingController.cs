using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PathfindingController : MonoBehaviour
{
    public AStar astar;

    public WalkData data;

    private Stack<AStarSharp.Node> nodes;
    public Character character;

    private float timeLeftOnNode;
    private static float maxTimeOnNode = 1f;

    private Vector2Int movingToTile;
    public Vector2Int Target
    {
        get
        {
            if (!IsMoving)
                return GetCurrentTileId;

            return movingToTile;
        }
    }
   
    private void Start()
    {
        if (character == null)
            character = GetComponent<Character>();
        if (astar == null)
            astar = FindAStar();
    }

    public bool MoveTo(Vector2 position)
    {
        return MoveToId(astar.GetTileId(position));
    }

    public bool MoveToId(Vector2Int id)
    {
        var start = GetCurrentTileId;
        if (character.IsGrounded && !CanWalkOnTile(start))
        {
            // scenario when character is standing on corner of tile
            if (CanWalkOnTile(start - new Vector2Int(1, 0)))
                start = start - new Vector2Int(1, 0);
            else if (CanWalkOnTile(start + new Vector2Int(1, 0)))
                start = start + new Vector2Int(1, 0);
        }

        var end = id;

        nodes = astar.GetPath(start, end, data);
        if (nodes == null)
        {
            return false;
        }

        movingToTile = id;


        timeLeftOnNode = maxTimeOnNode;

        return true;
    }

    public void StopMoving()
    {
        if (nodes != null)
            nodes.Clear();
    }

    public bool IsMoving { get; set; }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (timeLeftOnNode < 0 && nodes != null)
        {
            nodes.Clear();
        }

        if (nodes != null && nodes.Count > 0)
        {
            timeLeftOnNode -= Time.fixedDeltaTime;

            var id = nodes.Peek().Id;
            var current = GetCurrentTileId;
            if (id.x == current.x && Mathf.Abs(id.y - current.y) <=1)
            {
                nodes.Pop();
                timeLeftOnNode = maxTimeOnNode;
            }

            Vector2 move = new Vector2();

            if (id.x > current.x)
                move.x = 1;
            else if (id.x < current.x)
                move.x = -1;

            if (id.y > current.y)
                move.y = 1;
            if (id.y < current.y)
                move.y = -1;

            character.Move = move;

            if (nodes.Count == 0)
                character.Move = new Vector2();

        }
    }

    private void OnDrawGizmos()
    {
        if (astar == null || nodes == null)
            return;
        foreach (var node in nodes)
            DrawNode(node);
    }

    private void DrawNode(AStarSharp.Node node)
    {
  
        var pos = astar.GetPositionFromId(node.Id);
        if (node.jumpDistanceLeft > 0 || node.jumpHeightLeft > 0)
            Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(pos.x, pos.y, 0) ,0.2f);
        Gizmos.color = Color.white;
    }

    public bool CanWalkOnPosition(Vector2 position)
    {
        return CanWalkOnTile(GetTileId(position));
    }
    public bool CanWalkOnTile(Vector2Int Id)
    {
        for (int i = Id.y + 1; i < Id.y + data.height; i++)
        {
            var above = astar.GetNode(new Vector2Int(Id.x, i));
            if (above.Block)
                return false;
        }

        var floor = astar.GetNode(new Vector2Int(Id.x, Id.y - 1));

        return floor.Block || floor.Platform;
    }

    public Vector2Int GetTileId(Vector2 position)
    {
        return astar.GetTileId(position + new Vector2(0, 0.1F));
    }

    public Vector2Int GetCurrentTileId { get => GetTileId(transform.position); }

    private AStar FindAStar()
    {
        GameObject parent = gameObject;
        do
        {
            parent = parent.transform?.parent?.gameObject;
            var astar = parent.GetComponent<AStar>();
            if (astar == null)
                continue;
            return astar;

        }
        while (parent != null);
        Debug.LogWarning("AStar not found");
        return null;
    }
}