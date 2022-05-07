
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
    public float ClimbLadderSpeed = 0;
    public float CoyoteTime = 0.15f;
    public float WalkSpeed = 10;
    public float JumpSpeed = 20;
    public float Acceleration = 10;
    public bool CanUseLadder = true;
    public bool FaceLeft = false;

    public float StepTime = 0.15f;

    public UnityEvent OnJump;
    public UnityEvent OnWalk;

    // on which platforms or ladders character currently is
    public HashSet<LadderTile> ladders = new HashSet<LadderTile>();
    public HashSet<PlatformTile> platforms = new HashSet<PlatformTile>();

    public Vector2 move = new Vector2();

    private Character character;
    private Rigidbody2D rigidbody;
    private Collider2D collider2D;

    private float timeToFinishCoyoteTime = 0;
    private float timeToStep = 0;

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

    public bool IsGrounded
    {
        get
        {
            if (System.Math.Round(rigidbody.velocity.y, 1) != 0)
                return false;
            var raycast = Physics2D.BoxCastAll(collider2D.bounds.center, collider2D.bounds.size + new Vector3(-0.01f, -0.1f), 0, Vector2.down, 0.15f);
            foreach (var cast in raycast)
            {
                if (cast.collider.gameObject != gameObject
                    && !cast.collider.isTrigger
                    && !Physics2D.GetIgnoreLayerCollision(cast.collider.gameObject.layer, gameObject.layer))
                    return true;
            }
            return false;
        }
    }

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

    public void FixedUpdate()
    {
        if (character.holdUpdate)
            return;

        if (character.IsDead)
        {
            move = new Vector2();
            //ChargeAttack = false;
        }

        if (FaceLeft)
            transform.localScale = new Vector2(-1, 1);
        else
            transform.localScale = new Vector2(1, 1);

        if (rigidbody.velocity.x > WalkSpeed * move.x && move.x <= 0)
        {
            float additionalSpeed = Acceleration * Time.fixedDeltaTime;
            rigidbody.velocity = new Vector2(Mathf.Max(WalkSpeed * move.x, rigidbody.velocity.x - additionalSpeed), rigidbody.velocity.y);
        }

        // move right
        if (rigidbody.velocity.x < WalkSpeed * move.x && move.x >= 0)
        {
            float additionalSpeed = Acceleration * Time.fixedDeltaTime;
            rigidbody.velocity = new Vector2(Mathf.Min(WalkSpeed * move.x, rigidbody.velocity.x + additionalSpeed), rigidbody.velocity.y);
        }

        // stop
        if (move.x > 0)
            FaceLeft = false;

        if (move.x < 0)
            FaceLeft = true;

        if (Flying)
        {
            if (rigidbody.velocity.y > WalkSpeed * move.y && move.y <= 0)
            {
                float additionalSpeed = Acceleration * Time.fixedDeltaTime;
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Max(WalkSpeed * move.y, rigidbody.velocity.y - additionalSpeed));
            }

            if (rigidbody.velocity.y < WalkSpeed * move.y && move.y >= 0)
            {
                float additionalSpeed = Acceleration * Time.fixedDeltaTime;
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Min(WalkSpeed * move.y, rigidbody.velocity.y + additionalSpeed));
            }
        }
        else
        {

            if (ladders.Count == 0 && CanUseLadder)
                IsOnLadder = false;

            if (move.y != 0 && ladders.Count > 0 && CanUseLadder)
                IsOnLadder = true;

            bool isGrounded = IsGrounded;
            if (isGrounded)
            {
                IsJumping = false;
                timeToFinishCoyoteTime = CoyoteTime;
            }
            else
                timeToFinishCoyoteTime -= Time.fixedDeltaTime;

            bool canJump = (isGrounded || timeToFinishCoyoteTime > 0) && !IsJumping;


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

            if (isGrounded && move.x != 0)
            {
                timeToStep -= Time.fixedDeltaTime;
                if (timeToStep < 0)
                {
                    timeToStep = StepTime;
                    OnWalk.Invoke();
                }
            }
        }
    }

    public void Teleport(Vector2 position)
    {
        StartCoroutine(TeleportCoroutine(position));
    }

    private IEnumerator TeleportCoroutine(Vector2 position)
    {
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

