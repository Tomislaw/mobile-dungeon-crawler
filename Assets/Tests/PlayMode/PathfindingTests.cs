using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static AStarSharp.Astar;

public class PathfindingTests : TestSceneTests
{
    // Test can find a path slightly to the right
    [UnityTest]
    public IEnumerator BasicPathfindingSuccessPath()
    {
        PathfindingController pCtrl = skelly.GetComponent<PathfindingController>();

        //TODO replace with test marker gameobject
        Vector2Int easyTarget = pCtrl.GetCurrentTileId + new Vector2Int(2, 0);
        IEnumerator<FindPathStatus> enumerator = pCtrl.astar.GetPath(pCtrl.GetCurrentTileId, easyTarget, pCtrl.data);
        Stack<AStarSharp.Node> nodes = waitForPath(enumerator);

        Assert.Greater(nodes.Count, 1, "Too few nodes found");

        yield return null;
    }

    // Test can't find a path directly down through the ground
    [UnityTest]
    public IEnumerator BasicPathfindingFailPath()
    {
        PathfindingController pCtrl = skelly.GetComponent<PathfindingController>();

        //TODO replace with test marker gameobject
        Vector2Int easyTarget = pCtrl.GetCurrentTileId + new Vector2Int(0, -2);
        IEnumerator<FindPathStatus> enumerator = pCtrl.astar.GetPath(pCtrl.GetCurrentTileId, easyTarget, pCtrl.data);
        Stack<AStarSharp.Node> nodes = waitForPath(enumerator);

        Assert.AreEqual(nodes.Count, 0, "Path was found when it should not have");

        yield return null;
    }
}
