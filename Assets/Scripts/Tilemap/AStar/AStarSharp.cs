using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RuinsRaiders.AStarSharp
{

    public struct Tile
    {
        public Vector2Int Id;
        public bool Spike;
        public bool Block;
        public bool Slope;
        public bool Water;
        public bool Ladder;
        public bool Platform;
        public bool Destroyable;
    }

    public class Node
    {
        public Node Parent;
        public Vector2Int Id;
        public Tile Tile;

        public float DistanceToTarget;
        public float Cost;
        public float Weight;

        public Node(Tile Tile)
        {
            this.Tile = Tile;
            Id = Tile.Id;
        }

        public float F
        {
            get
            {
                if (DistanceToTarget != -1 && Cost != -1)
                    return DistanceToTarget + Cost;
                else
                    return -1;
            }
        }

        public Type action = Type.None;
        public Type nextAction = Type.None;

        public int jumpHeightLeft = -1;
        public int jumpDistanceLeft = -1;

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var compare = obj as Node;
            return this.Id == compare.Id
                && this.jumpHeightLeft == compare.jumpHeightLeft
                && this.jumpDistanceLeft == compare.jumpDistanceLeft
                && this.action == compare.action;
        }

        public Node Clone()
        {
            return (Node)this.MemberwiseClone();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public enum Type
        {
            None, Walk, Jump, Fall, Drop, Climb, Swim, Fly
        }
    }

    public class Astar
    {
        public Func<Vector2Int, Tile> GetTile;
        public Func<Node, List<Node>> GetAdjacentNodes;
        public Func<Node, float> GetCost;
        public Func<Node, float> GetDistance;
        public Func<Node, bool> IsEnd;

        public int SizePerBatch = 10;
        public int MaxSize = 600;

        public struct FindPathStatus
        {
            public bool Finished;
            public bool PathFound;
            public Stack<Node> Path;
        }

        public IEnumerator<FindPathStatus> FindPath(
            Vector2Int Start,
            CancellationToken ct = default)
        {
            Node start = new Node(GetTile(Start));

            Stack<Node> Path = new();
            List<Node> OpenList = new();
            List<Node> ClosedList = new();
            List<Node> adjacencies;
            Node current = start;

            // add start node to Open List
            OpenList.Add(start);

            while (OpenList.Count != 0 && !ClosedList.Exists(x => IsEnd(x)) && ClosedList.Count < MaxSize)
            {
                if (ct.IsCancellationRequested || ClosedList.Count % SizePerBatch == 0)
                    yield return new FindPathStatus();

                current = OpenList[0];
                OpenList.Remove(current);
                ClosedList.Add(current);
                adjacencies = GetAdjacentNodes(current);

                foreach (Node n in adjacencies)
                {
                    if (ct.IsCancellationRequested)
                        yield return new FindPathStatus();

                    if (ClosedList.Contains(n))
                        continue;
                    if (OpenList.Contains(n))
                        continue;

                    n.Parent = current;
                    n.DistanceToTarget = GetDistance(n);
                    n.Cost = GetCost(n);
                    OpenList.Add(n);
                    OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();

                }
            };

            var pathFound = true;
            // construct path, if end was not closed return null
            if (!ClosedList.Exists(n => IsEnd(n)) && ClosedList.Count > 0)
            {
                current = ClosedList.Where(it => it.jumpHeightLeft == -1 && !it.Tile.Spike).OrderBy(n => GetDistance(n)).First();
                pathFound = false;
            }

            var previousAction = current;
            while(previousAction.Parent != null)
            {
                previousAction.Parent.nextAction = previousAction.action;
                previousAction = previousAction.Parent;
            }

            // if all good, return path
            Node temp = ClosedList[ClosedList.IndexOf(current)];
            if (temp == null)
            {
                yield return new FindPathStatus() { Finished = true, PathFound = false };
                yield break;
            }

            do
            {
                Path.Push(temp);
                temp = temp.Parent;
            } while (temp != start && temp != null);

            yield return new FindPathStatus() { Finished = true, PathFound = pathFound, Path = Path };
            yield break;

        }
    }
}