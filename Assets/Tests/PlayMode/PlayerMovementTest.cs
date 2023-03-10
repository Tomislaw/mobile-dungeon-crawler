using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovementTest : TestSceneTests
{

    [UnityTest]
    public IEnumerator MoveRight()
    {
        yield return new WaitForDone(waitTime, () => !mCtrl.IsMoving);
        var startPos = knight.transform.position;
        pCtrl.GetControlData().MoveRight(true);
        yield return new WaitForDone(waitTime, () => startPos.x < knight.transform.position.x);
        yield return new WaitForSeconds(0.1F); //Give it just a moment extra
        pCtrl.GetControlData().MoveRight(false);
        Assert.Less(startPos.x, knight.transform.position.x, "Player didn't move right");
        Assert.False(mCtrl.faceLeft, "Should be facing right");
    }

    [UnityTest]
    public IEnumerator MoveLeft()
    {
        yield return new WaitForDone(waitTime, () => !mCtrl.IsMoving);
        var startPos = knight.transform.position;
        pCtrl.GetControlData().MoveLeft(true);
        yield return new WaitForDone(waitTime, () => startPos.x > knight.transform.position.x);
        yield return new WaitForSeconds(0.1F); //Give it just a moment extra
        pCtrl.GetControlData().MoveLeft(false);
        Assert.Greater(startPos.x, knight.transform.position.x, "Player didn't move left");
        Assert.True(mCtrl.faceLeft, "Should be facing left");
    }

    [UnityTest]
    public IEnumerator Jump()
    {
        yield return new WaitForDone(waitTime, () => !mCtrl.IsMoving && mCtrl.IsGrounded);
        var startPos = knight.transform.position;
        pCtrl.GetControlData().MoveUp(true);
        yield return new WaitForDone(waitTime, () => mCtrl.IsJumping);
        pCtrl.GetControlData().MoveUp(false);
        Assert.Less(startPos.y, knight.transform.position.y, "Player didn't jump");
        Assert.True(mCtrl.IsJumping, "Player isn't in jumping state");
    }

    [UnityTest]
    public IEnumerator Fall()
    {
        yield return new WaitForDone(waitTime, () => !mCtrl.IsMoving && mCtrl.IsGrounded);
        var startPos = knight.transform.position;
        pCtrl.GetControlData().MoveDown(true);
        yield return new WaitForDone(waitTime, () => !mCtrl.IsGrounded);
        pCtrl.GetControlData().MoveDown(false);
        Assert.Greater(startPos.y, knight.transform.position.y, "Player didn't fall");
        Assert.False(mCtrl.IsGrounded, "Player was grounded when it should be falling");
    }
}
