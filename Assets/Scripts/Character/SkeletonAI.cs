using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    public List<string> friends;

    private Character character;
    private PathfindingController pathfinding;

    private Character target = null;

    public State state;

    private static float thinkTime = 0.1f;
    private Thread thread;

    private void Start()
    {
        character = GetComponent<Character>();
        pathfinding = GetComponent<PathfindingController>();
        timeToAttack = timeBetweenAttacks;

    }

    private void Destroy()
    {
        try
        {
            thread.Abort();
        } catch(ThreadAbortException e)
        {
            Debug.LogError(e);
        }
    }

    // Update is called once per frame
    private void Think()
    {
        if (character.IsDead)
            return;

        switch (state)
        {
            case State.Idle:

                // search for enemies
                timeToSearch -= thinkTime;
                if (timeToSearch < 0)
                {
                    timeToSearch = timeBetweenSearch;
                    if (SearchForEnemies())
                    {
                        Stop();
                        state = State.Alerted;
                        break;
                    }
                }

                // walk to random tile
                timeToWalk -= thinkTime;
                if (timeToWalk < 0)
                {
                    timeToWalk = timeBetweenWalks;
                    Stop();
                    WalkToRandom();
                }
                break;

            case State.Alerted:

                if (target == null || target.IsDead) // back to idle when target dead
                {
                    target = null;
                    state = State.Idle;
                    Stop();
                    break;
                }

                var distance = Vector2.Distance(target.transform.position, transform.position);

                if (distance > 10) // back to idle when target far away or dead
                {
                    target = null;
                    state = State.Idle;
                    Stop();
                    break;
                }
                else if (distance < 1.5) // attack when close
                {
                    state = State.Attacking;
                    Stop();
                    break;
                }
                else  // chase enemy
                {
                    timeToRefreshPath -= thinkTime;
                    if (timeToRefreshPath < 0)
                    {
                        timeToRefreshPath = timeBetweenRefresh;
                        Stop();
                        Chase();
                    }
                }

                break;

            case State.Attacking:

                if (target == null || target.IsDead)
                {
                    state = State.Idle;
                    character.Move = new Vector2();
                    character.ChargeAttack = false;
                    break;
                }

                timeToAttack -= thinkTime;
                if (timeToAttack < 0)
                {
                    timeToAttack = timeBetweenAttacks;
                    Attack();
                    Stop();

                    var dist = Vector2.Distance(target.transform.position, transform.position);
                    if (dist > 1.5)
                    {
                        state = State.Alerted;
                        character.Move = new Vector2();
                        character.ChargeAttack = false;
                        break;
                    }
                }
                else
                {
                    if (!character.ChargeAttack)
                    {
                        PreAttack();
                        Stop();
                    }
                }

                break;
        }
    }

    private void FixedUpdate()
    {
        thinkTime = Time.fixedDeltaTime;
        Think();
    }

    public float searchDistance = 10f;
    public float searchHeight = 2f;
    public float timeBetweenSearch = 0.5f;
    private float timeToSearch;

    public bool SearchForEnemies()
    {
        var raycast = Physics2D.OverlapBoxAll(transform.position, new Vector2(searchDistance * 2, searchHeight * 2), 0);
        foreach (var go in raycast)
        {
            var ch = go.gameObject.GetComponent<Character>();

            if (ch != null && !ch.IsDead && ch.group != character.group)
            {
                target = ch;
                return true;
            }
        }
        return false;
    }

    public float timeBetweenWalks = 5;
    private float timeToWalk;

    public void WalkToRandom()
    {
        var id = pathfinding.GetCurrentTileId;
        int maxRetries = 3;
        for (int i = 0; i < maxRetries; i++)
        {
            var pos = new Vector2Int(id.x  + Random.Range(-3,4), id.y);

            var walk = pathfinding.MoveToId(pos);
            if (walk)
                break;
        }
    }

    public float timeBetweenRefresh = 1;
    private float timeToRefreshPath;

    public void Chase()
    {
        if (target == null)
            return;

        pathfinding.MoveTo(target.transform.position);
    }

    private float timeToAttack;
    public float timeBetweenAttacks = 0.5f;

    public void Attack()
    {
        //character.FaceLeft = target.transform.position.x < transform.position.x;
        character.Attack();
    }

    public void PreAttack()
    {
        character.FaceLeft = target.transform.position.x < transform.position.x;
        character.ChargeAttack = true;
    }

    public void Stop()
    {
        character.Move = new Vector2(0, 0);
        pathfinding.StopMoving();
    }

    public enum State
    {
        Idle,
        Alerted,
        Attacking
    }
}