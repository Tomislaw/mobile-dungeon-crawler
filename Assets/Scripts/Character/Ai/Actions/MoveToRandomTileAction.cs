using Assets.Scripts.Character.Ai;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MoveToRandomTileAction", menuName = "RuinsRaiders/Ai/MoveToRandomTileAction", order = 1)]
public class MoveToRandomTileAction : BasicAiActionData
{
    public int searchDistanceX = 4;
    public int searchDistanceY = 1;

    public int NumberOfRetries = 5;
    public float AwaitTime = 1;

    public override BasicAiAction Create(ActivatorData trigger)
    {
        return new Action(trigger, this);
    }

    public class Action : BasicAiAction
    {
        private Character character;
        private PathfindingController pathfinding;
        private float awaitTime;
        public Action(ActivatorData trigger, MoveToRandomTileAction parent)
        {
            character = trigger.triggeredFor.GetComponent<Character>();
            pathfinding = trigger.triggeredFor.GetComponent<PathfindingController>();
            awaitTime = parent.AwaitTime;
            var id = pathfinding.GetCurrentTileId;
            for (int i = 0; i < parent.NumberOfRetries; i++)
            {
                var pos = new Vector2Int(
                    id.x + Random.Range(-parent.searchDistanceX, parent.searchDistanceX + 1),
                    id.y + Random.Range(-parent.searchDistanceY, parent.searchDistanceY + 1));

                if (!pathfinding.CanWalkOnTile(pos))
                    continue;

                if (pathfinding.MoveToId(pos))
                    break;
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
    }
}

