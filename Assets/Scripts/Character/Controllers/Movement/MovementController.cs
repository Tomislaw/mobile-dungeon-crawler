
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Character))]
public class MovementController : MonoBehaviour
{

    public bool Flying = false;

    // movement properties
    public float GravityScale = 6;

    public float ClimbLadderSpeed = 0;
    public float CoyoteTime = 0.15f;
    public float WalkSpeed = 10;
    public float JumpSpeed = 20;
    public float MaxSpeed = 30;
    public float Acceleration = 10;
    public float Jerk = 10;
    public bool CanUseLadder = true;
    public bool FaceLeft = false;

    public float StepTime = 0.15f;

    public UnityEvent OnJump;
    public UnityEvent OnWalk;
    public UnityEvent OnSwim;

    // on which platforms or ladders character currently is
    public HashSet<LadderTile> ladders = new HashSet<LadderTile>();
    public HashSet<PlatformTile> platforms = new HashSet<PlatformTile>();

    public Vector2 move = new Vector2();
    private Vector2 previousMove = new Vector2();

    private Character character;
    private Rigidbody2D rigidbody;
    private Collider2D collider2D;

    private float timeToFinishCoyoteTime = 0;
    private float timeToStep = 0;

    private float accumulatedAcceleration;

    public bool IsJumping { get; private set; }
    public bool IsMoving { get => move.x != 0; }
    public bool IsOnLadder {get; private set;}
    public bool IsColliderAbove
    {
        get
        {
            var raycast = Physics2D.LinecastAll(transform.position, transform.position + new Vector3(0, 1.1f, 0));
            foreach (var cast in raycast)
            {
                if (cast.collider is TilemapCollider2D || cast.collider is CompositeCollider2D)
                    return true;
            }
            return false;
        }
    }

    public bool IsGrounded { get; private set; }

    public void Move(Vector2 moveDirections)
    {
        move = moveDirections;
    }

    public void Stop()
    {
        move = new Vector2();
    }

    public void OnEnable()
    {
        character = GetComponent<Character>();
        collider2D = GetComponent<Collider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private Vector2 raycastVector;

    private bool CheckForGround()
    {
        //if (System.Math.Round(rigidbody.velocity.y, 1) != 0)
        //    return false;
        var raycast = Physics2D.BoxCastAll(transform.position + new Vector3(0,0.1f,0), 
              new Vector3(collider2D.bounds.size.x - 0.02f, 0.01f), 0, Vector2.down, 0.2f);
        foreach (var cast in raycast)
        {
            if (cast.collider.gameObject != gameObject
                && !cast.collider.isTrigger
                && !Physics2D.GetIgnoreLayerCollision(cast.collider.gameObject.layer, gameObject.layer))
                return true;
        }
        return false;
    }

    public void FixedUpdate()
    {
        if (character.holdUpdate)
            return;

        if (character.IsDead)
            Stop();

        var landed = IsGrounded;
        IsGrounded = CheckForGround();
        landed = !landed && IsGrounded;

        // change scale based on face direction
        if (FaceLeft)
            transform.localScale = new Vector2(-1, 1);
        else
            transform.localScale = new Vector2(1, 1);

        if (move.x == 0 || Mathf.Sign(previousMove.x) != Mathf.Sign(move.x) )
            accumulatedAcceleration = Acceleration;
        else
            accumulatedAcceleration += Jerk * Time.fixedDeltaTime;

        // set direction based on movement
        if (move.x > 0)
            FaceLeft = false;

        if (move.x < 0)
            FaceLeft = true;

        // if flying use diffirent behavior
        if (Flying)
        {
            if (rigidbody.velocity.y > WalkSpeed * move.y && move.y <= 0)
            {
                float additionalSpeed = accumulatedAcceleration * Time.fixedDeltaTime;
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Max(WalkSpeed * move.y, rigidbody.velocity.y - additionalSpeed));
            }

            if (rigidbody.velocity.y < WalkSpeed * move.y && move.y >= 0)
            {
                float additionalSpeed = accumulatedAcceleration * Time.fixedDeltaTime;
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Min(WalkSpeed * move.y, rigidbody.velocity.y + additionalSpeed));
            }
        }
        else
        {

            if (ladders.Count == 0 && CanUseLadder)
                IsOnLadder = false;

            if (move.y != 0 && ladders.Count > 0 && CanUseLadder)
                IsOnLadder = true;

            if (IsGrounded)
            {
                IsJumping = false;
                timeToFinishCoyoteTime = CoyoteTime;
            }
            else
                timeToFinishCoyoteTime -= Time.fixedDeltaTime;

            // move on ground and slopes
            var slope = GetSlopeFactor();
            if (landed)
            {
                // todo: special behavior on landing on slopes
            }
            bool onSlope = slope.x < 0.99f;

            bool canJump = (IsGrounded || timeToFinishCoyoteTime > 0) 
                && !IsJumping 
                && !IsColliderAbove
                && (onSlope || rigidbody.velocity.y <= 0.1f);


            if (IsOnLadder && CanUseLadder)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, ClimbLadderSpeed * move.y);
            }
            else if (canJump && move.y > 0)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, JumpSpeed * move.y);
                IsJumping = true;
                OnJump.Invoke();
            }

            // get move direction x
            int moveDir = 0;
            if (rigidbody.velocity.x > WalkSpeed * move.x && move.x <= 0)
                moveDir = -1;
            else if (rigidbody.velocity.x < WalkSpeed * move.x && move.x >= 0)
                moveDir = 1;


            if (moveDir != 0)
            {
                float speed = accumulatedAcceleration * Time.fixedDeltaTime * moveDir;
                Vector2 additionalSpeed = new Vector2(speed * slope.x, speed * slope.y);
                AddSpeed(additionalSpeed, WalkSpeed*Mathf.Abs(move.x));
            }

            if (onSlope || IsOnLadder)
                rigidbody.gravityScale = 0;
            else
                rigidbody.gravityScale = GravityScale;

            if (IsGrounded && move.x != 0)
            {
                timeToStep -= Time.fixedDeltaTime;
                if (timeToStep < 0)
                {
                    timeToStep = StepTime;
                    OnWalk.Invoke();
                }
            }
        }

        previousMove = move;

        if(rigidbody.velocity.magnitude>MaxSpeed)
            rigidbody.velocity = rigidbody.velocity.normalized * MaxSpeed;
    }

    private Vector2 GetSlopeFactor()
    {

        if (IsJumping)
            return new Vector2(1, 0);

        var center = collider2D.bounds.center;
        var height = collider2D.bounds.size.y;

        var hit1 = Physics2D.Raycast(center, Vector2.down, height, Physics2D.GetLayerCollisionMask(collider2D.gameObject.layer));

        float dir = FaceLeft ? -1 : 1;
        center.x += collider2D.bounds.size.x * dir /2f;
        center.y -= 0.001f;

        var hit2 = Physics2D.Raycast(center, Vector2.down, height, Physics2D.GetLayerCollisionMask(collider2D.gameObject.layer));

        if (hit1 && hit2)
        {
            return new Vector2(Mathf.Max(hit1.normal.y, hit2.normal.y), -Mathf.Min(hit1.normal.x * dir, hit2.normal.x * dir) * dir);
        }
        else if (hit1)
            return new Vector2(hit1.normal.y, -hit1.normal.x );
        else if (hit2)
            return new Vector2(hit2.normal.y, -hit2.normal.x );

        return new Vector2(1, 0);
    }

    private void AddSpeed(Vector2 addSpeed, float limit)
    {
        
        AddSpeed(addSpeed, new Vector2(limit, limit));
    }
    private void AddSpeed(Vector2 addSpeed, Vector2 limit)
    {
        AddSpeed(addSpeed, -limit, limit);
    }
    private void AddSpeed(Vector2 addSpeed, Vector2 limitMin, Vector2 limitMax)
    {
        Vector2 newSpeed = rigidbody.velocity;
        if (addSpeed.x < 0 && rigidbody.velocity.x > limitMin.x)
        {
            newSpeed.x += addSpeed.x;
            if(newSpeed.x < limitMin.x)
                    newSpeed.x = limitMin.x;
        }
        if (addSpeed.x > 0 && rigidbody.velocity.x < limitMax.x)
        {
            newSpeed.x += addSpeed.x;
            if (newSpeed.x > limitMax.x)
                newSpeed.x = limitMax.x;
        }
        if (addSpeed.y < 0 && rigidbody.velocity.y > limitMin.y)
        {
            newSpeed.y += addSpeed.y;
            if (newSpeed.y < limitMin.y)
                newSpeed.y = limitMin.y;
        }
        if (addSpeed.y > 0 && rigidbody.velocity.y < limitMax.y)
        {
            newSpeed.y += addSpeed.y;
            if (newSpeed.y > limitMax.y)
                newSpeed.y = limitMax.y;
        }
        rigidbody.velocity = newSpeed;
    }

    public void Teleport(Vector2 position)
    {
        StartCoroutine(TeleportCoroutine(position));
    }

    private IEnumerator TeleportCoroutine(Vector2 position)
    {
        if (rigidbody)
            rigidbody.velocity = new Vector2();
        character.holdUpdate = true;
        transform.position = position;
        character.SetAnimation("Dead");
        yield return new WaitForSeconds(0.3f);
        character.SetAnimation("Idle");
        character.holdUpdate = false;
    }

    public void FacePosition(Vector2 position)
    {
        FaceLeft = position.x < transform.position.x;
    }
}

