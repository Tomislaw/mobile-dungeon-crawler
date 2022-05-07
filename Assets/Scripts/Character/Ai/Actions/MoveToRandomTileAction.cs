using Assets.Scripts.Character.Ai;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "MoveToRandomTileAction", menuName = "RuinsRaiders/Ai/MoveToRandomTileAction", order = 1)]
public class MoveToRandomTileAction : BasicAiActionData
{
    public int searchDistanceX = 4;
    public int searchDistanceY = 1;

    public float AwaitTime = 1;

    public override BasicAiAction Create(ActivatorData trigger)
    {
        return new Action(trigger, this);
    }

    public class Action : BasicAiAction
    {
        private MovementController character;
        private PathfindingController pathfinding;
        private float awaitTime;
        public Action(ActivatorData trigger, MoveToRandomTileAction parent)
        {
            character = trigger.triggeredFor.GetComponent<MovementController>();
            pathfinding = trigger.triggeredFor.GetComponent<PathfindingController>();
            awaitTime = parent.AwaitTime;
            var id = pathfinding.GetCurrentTileId;

            List<Vector2Int> positions = new List<Vector2Int>();
            for(int x = -parent.searchDistanceX; x <= parent.searchDistanceX; x++)
                for (int y = -parent.searchDistanceY; y <= parent.searchDistanceY; y++)
                {
                    if (id.x == 0 && id.y == 0)
                        continue;

                    var pos = new Vector2Int(id.x + x, id.y + y);
                    if (pathfinding.CanWalkOnTile(pos))
                        positions.Add(pos);
                }

            var randomizedPositions = positions.OrderBy(it=>Random.Range(0f, 1f));
            foreach (var pos in randomizedPositions)
            {
                if (pathfinding.MoveToId(pos))
                    return;
            }
        }


        public override bool CanStop()
        {
            return IsFinished() || character.IsGrounded;
        }
        public override bool IsFinished()
        {
            return !pathfinding.IsMoving && awaitTime <=0;
        }


        public override void Update(float dt)
        {
            if (!pathfinding.IsMoving && awaitTime > 0)
                awaitTime -= dt;
        }

        public override string ToString()
        {
            return "MoveToRandomTileAction;" +
                "\nawait: " + awaitTime.ToString() +
                "\nmoving: " + pathfinding.IsMoving +
                "\nnodes: " + pathfinding.RemainingNodes() +
                "\nfinished: " + IsFinished();
        }
    }
}

