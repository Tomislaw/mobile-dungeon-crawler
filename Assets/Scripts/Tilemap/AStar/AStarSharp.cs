using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RuinsRaiders.AStarSharp
{
    public class Node
    {
        public Node Parent;
        public Vector2Int Id;

        public float DistanceToTarget;
        public float Cost;
        public float Weight;

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

        public bool Spike;
        public bool Block;
        public bool Water;
        public bool Ladder;
        public bool Platform;
        public bool Destroyable;
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
                && this.jumpDistanceLeft == compare.jumpDistanceLeft;
        }

        public Node Clone()
        {
            return (Node)this.MemberwiseClone();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Astar
    {
        public Func<Vector2Int, Node> GetNode;
        public Func<Node, List<Node>> GetAdjacentNodes;

        public struct FindPathStatus
        {
            public bool Finished;
            public bool PathFound;
            public Stack<Node> Path;
        }

        public IEnumerator<FindPathStatus> FindPath(Vector2Int Start, Vector2Int End, int SizePerBatch = 10, int MaxSize = 600, CancellationToken ct = default)
        {
            Node start = GetNode(Start);
            Node end = GetNode(End);

            Stack<Node> Path = new();
            List<Node> OpenList = new();
            List<Node> ClosedList = new();
            List<Node> adjacencies;
            Node current = start;

            // add start node to Open List
            OpenList.Add(start);

            while (OpenList.Count != 0 && !ClosedList.Exists(x => x.Id == end.Id) && ClosedList.Count < MaxSize)
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
                    n.DistanceToTarget = Vector2.Distance(n.Id, end.Id);
                    n.Cost = n.Weight + n.Parent.Cost;
                    OpenList.Add(n);
                    OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();

                }
            };

            var pathFound = true;
            // construct path, if end was not closed return null
            if (!ClosedList.Exists(x => x.Id == end.Id))
            {
                current = ClosedList.Where(it => it.jumpHeightLeft == -1 && !it.Spike).OrderBy(it => Vector2Int.Distance(it.Id, End)).First();
                pathFound = false;
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


        public (bool, Stack<Node>) FindPath(Vector2Int Start, Vector2Int End, int MaxSize = 600, CancellationToken ct = default)
        {
            Node start = GetNode(Start);
            Node end = GetNode(End);

            Stack<Node> Path = new();
            List<Node> OpenList = new();
            List<Node> ClosedList = new();
            List<Node> adjacencies;
            Node current = start;

            // add start node to Open List
            OpenList.Add(start);

            while (OpenList.Count != 0 && !ClosedList.Exists(x => x.Id == end.Id) && ClosedList.Count < MaxSize)
            {
                if (ct.IsCancellationRequested)
                    return (false, null);
                current = OpenList[0];
                OpenList.Remove(current);
                ClosedList.Add(current);
                adjacencies = GetAdjacentNodes(current);

                foreach (Node n in adjacencies)
                {
                    if (ct.IsCancellationRequested)
                        return (false, null);
                    if (ClosedList.Contains(n))
                        continue;
                    if (OpenList.Contains(n))
                        continue;

                    n.Parent = current;
                    n.DistanceToTarget = Vector2.Distance(n.Id, end.Id);
                    n.Cost = n.Weight + n.Parent.Cost;
                    OpenList.Add(n);
                    OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();
                }
            }

            if (ct.IsCancellationRequested)
                return (false, null);


            // construct path, if end was not closed return null
            if (!ClosedList.Exists(x => x.Id == end.Id))
            {
                current = ClosedList.Where(it => it.jumpHeightLeft == -1).OrderBy(it => Vector2Int.Distance(it.Id, End)).First();
            }

            // if all good, return path
            Node temp = ClosedList[ClosedList.IndexOf(current)];
            if (temp == null) return (false, null);
            do
            {
                if (ct.IsCancellationRequested)
                    return (false, null);

                Path.Push(temp);
                temp = temp.Parent;
            } while (temp != start && temp != null);
            return (true, Path);
        }
    }
}