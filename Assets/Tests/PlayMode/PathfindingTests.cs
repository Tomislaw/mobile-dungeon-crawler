using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using RuinsRaiders;
using UnityEngine;
using UnityEngine.TestTools;
using static RuinsRaiders.AStarSharp.Astar;

public class PathfindingTests : TestSceneTests
{
    private bool CanReachPath(PathfindingController source, PathfindingController target)
    {
        IEnumerator<FindPathStatus> enumerator = source.GetAStar().GetPath(source.TileId, target.TileId, source.GetWalkData());
        Stack<RuinsRaiders.AStarSharp.Node> nodes = waitForPath(enumerator);
        return nodes.Count > 0;
    }

    // Test can find a path slightly to the right
    [UnityTest]
    public IEnumerator BasicPathfindingSuccessPath()
    {
        bool canReach = CanReachPath(skelly1.GetComponent<PathfindingController>(), skelly2.GetComponent<PathfindingController>());
        Assert.That(canReach, "Path to target not found");

        yield return null;
    }

    // Test can't find a path directly down through the ground
    [UnityTest]
    public IEnumerator BasicPathfindingFailPath()
    {
        PathfindingController pCtrl = skelly1.GetComponent<PathfindingController>();

        Vector2Int easyTarget = pCtrl.TileId + new Vector2Int(0, -2);
        IEnumerator<FindPathStatus> enumerator = pCtrl.GetAStar().GetPath(pCtrl.TileId, easyTarget, pCtrl.GetWalkData());
        Stack<RuinsRaiders.AStarSharp.Node> nodes = waitForPath(enumerator);

        Assert.AreEqual(nodes.Count, 0, "Path was found when it should not have");

        yield return null;
    }

    // Test ladder use
    [UnityTest]
    public IEnumerator LadderUse()
    {
        //Skelly1 - can jump and use ladder
        bool canReach = CanReachPath(skelly1.GetComponent<PathfindingController>(), skelly3.GetComponent<PathfindingController>());
        Assert.That(canReach, "Path to target not found");

        //Skelly2 - can jump but not use ladder
        canReach = CanReachPath(skelly2.GetComponent<PathfindingController>(), skelly3.GetComponent<PathfindingController>());
        Assert.False(canReach, "Path was found when it should not have");

        //Skelly3 - can use ladder
        canReach = CanReachPath(skelly3.GetComponent<PathfindingController>(), skelly2.GetComponent<PathfindingController>());
        Assert.That(canReach, "Path to target not found");

        yield return null;
    }

    // Test jumping ability
    [UnityTest]
    public IEnumerator JumpingAbility()
    {
        //Skelly2 - can jump
        bool canReach = CanReachPath(skelly2.GetComponent<PathfindingController>(), skelly1.GetComponent<PathfindingController>());
        Assert.That(canReach, "Path to target not found");

        //Skelly2 - can't jump all the way to FlyingSkelly
        canReach = CanReachPath(skelly2.GetComponent<PathfindingController>(), flyingSkelly.GetComponent<PathfindingController>());
        Assert.False(canReach, "Path was found when it should not have");

        //Skelly4 - can't jump high enough, can't go under
        canReach = CanReachPath(skelly4.GetComponent<PathfindingController>(), skelly1.GetComponent<PathfindingController>());
        Assert.False(canReach, "Path was found when it should not have");

        yield return null;
    }

    // Test the power of flight!
    [UnityTest]
    public IEnumerator Flight()
    {
        //Can reach anywhere!
        bool canReach = CanReachPath(flyingSkelly.GetComponent<PathfindingController>(), skelly1.GetComponent<PathfindingController>());
        Assert.That(canReach, "Path to target not found");
        canReach = CanReachPath(flyingSkelly.GetComponent<PathfindingController>(), skelly2.GetComponent<PathfindingController>());
        Assert.That(canReach, "Path to target not found");
        canReach = CanReachPath(flyingSkelly.GetComponent<PathfindingController>(), skelly3.GetComponent<PathfindingController>());
        Assert.That(canReach, "Path to target not found");
        canReach = CanReachPath(flyingSkelly.GetComponent<PathfindingController>(), skelly4.GetComponent<PathfindingController>());
        Assert.That(canReach, "Path to target not found");

        yield return null;
    }
}
