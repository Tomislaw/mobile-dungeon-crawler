using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    // movement properties
    public float ClimbLadderSpeed = 0;
    public float CoyoteTime = 0.15f;
    public float WalkSpeed = 10;
    public float JumpSpeed = 20;
    public float Acceleration = 10;
    public bool CanUseLadder = true;

    public LayerMask platformLayer;

    // attack and damage group
    public float AttackSpeed = 0.2f;
    public Group group;

    // health
    public int Health = 4;
    public int MaxHealth = 4;

    // fields used for animations
    public bool HaveSneakAnimation = false;
    public float AttackAnimationTime = 0.15f;
    public float StepTime= 0.15f;


    // fields used for controllig character
    public Vector2 Move = new Vector2();
    public bool ChargeAttack = false;
    public bool FaceLeft = false;

    // private fields
    private float timeToNextAttack = 0;
    private float timeToAnimationAttackFinish = 0;
    private float timeToStep = 0;
    private float timeToFinishCoyoteTime = 0;
    private bool isJumping = false;
    public bool CanAttack { get => timeToNextAttack <= 0 && !IsDead; }

    // damaging controlles (weapon used, armor etc)
    public OnAttack AttackController;
    public OnDamage DamageController;

    // events
    public UnityEvent OnDeath;
    public UnityEvent OnDamage;
    public UnityEvent OnAttack;
    public UnityEvent OnJump;
    public UnityEvent OnWalk;

    public Collider2D collider2D;
    private Rigidbody2D rigidbody;
    private Animator animator;

    // on which platforms or ladders character currently is
    public HashSet<LadderTile> ladders = new HashSet<LadderTile>();
    public HashSet<PlatformTile> platforms = new HashSet<PlatformTile>();

    public enum Group
    {
        Skeletons, Player, Goblins
    }

    private void Start()
    {
        if (collider2D == null)
            collider2D = GetComponent<Collider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (DamageController == null)
            DamageController = new DefaultDamageController(this);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isTeleporting)
            return;

        if (IsDead)
        {
            Move = new Vector2();
            ChargeAttack = false;
        }

        if (FaceLeft)
            transform.localScale = new Vector2(-1, 1);
        else
            transform.localScale = new Vector2(1, 1);

        if (rigidbody.velocity.x > WalkSpeed * Move.x && Move.x <= 0)
        {
            float additionalSpeed = Acceleration * Time.fixedDeltaTime;
            rigidbody.velocity = new Vector2(Mathf.Max(WalkSpeed * Move.x, rigidbody.velocity.x - additionalSpeed), rigidbody.velocity.y);
        }

        // move right
        if (rigidbody.velocity.x < WalkSpeed * Move.x && Move.x >= 0)
        {
            float additionalSpeed = Acceleration * Time.fixedDeltaTime;
            rigidbody.velocity = new Vector2(Mathf.Min(WalkSpeed * Move.x, rigidbody.velocity.x + additionalSpeed), rigidbody.velocity.y);
        }

        // stop

        if (Move.x > 0)
            FaceLeft = false;

        if (Move.x < 0)
            FaceLeft = true;

        if (ladders.Count == 0 && CanUseLadder)
            IsOnLadder = false;

        if (Move.y != 0 && ladders.Count > 0 && CanUseLadder)
            IsOnLadder = true;

        bool isGrounded = IsGrounded;
        if (isGrounded)
        {
            isJumping = false;
            timeToFinishCoyoteTime = CoyoteTime;
        }
        else
            timeToFinishCoyoteTime -= Time.fixedDeltaTime;

        bool canJump = (isGrounded || timeToFinishCoyoteTime > 0) && !isJumping;


        if (IsOnLadder && CanUseLadder)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, ClimbLadderSpeed * Move.y);
        }
        else if (canJump && Move.y > 0)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, JumpSpeed * Move.y);
            isJumping = true;
            OnJump.Invoke();
        }
           
        if(isGrounded && Move.x != 0)
        {
            timeToStep -= Time.fixedDeltaTime;
            if (timeToStep < 0)
            {
                timeToStep = StepTime;
                OnWalk.Invoke();
            }
        }

        string sneak = HaveSneakAnimation && IsColliderAbove ? "Sneak" : "";
        if (IsDead)
        {
            SetAnimation("Dead");
        }
        else if(ChargeAttack && CanAttack)
        {
            SetAnimation("PreAttack" + sneak);
        }
        else if (timeToAnimationAttackFinish > 0)
        {
            SetAnimation("Attack" + sneak);
        }
        else if (Move.x != 0 || rigidbody.velocity.x != 0)
        {
            SetAnimation("Walk" + sneak);
        }
        else if (IsDead)
        {
            SetAnimation("Dead");
        }
         
        else
            SetAnimation("Idle" + sneak);

        if (timeToAnimationAttackFinish > 0) timeToAnimationAttackFinish -= Time.deltaTime;
        if (timeToNextAttack > 0) timeToNextAttack -= Time.deltaTime;
        if (AttackController != null)
            AttackController.ChargeAttack = ChargeAttack;
    }

    public void Attack()
    {
        if (CanAttack)
        {
            timeToAnimationAttackFinish = AttackAnimationTime;
            timeToNextAttack = AttackSpeed;
            ChargeAttack = false;
            if (AttackController != null)
                AttackController.Attack();
        }
    }

    private string currentAnimation;

    public bool isTeleporting = false;

    public IEnumerator Teleport(Vector2 position)
    {
        isTeleporting = true;
        transform.position = position;
        SetAnimation("Dead");
        yield return new WaitForSeconds(0.3f);
        SetAnimation("Idle");
        isTeleporting = false;
    }

    public IEnumerator Hide()
    {
        isTeleporting = true;
        SetAnimation("Dead");
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }

    public void SetAnimation(string animation)
    {
        if (currentAnimation == animation)
            return;

        currentAnimation = animation;
        animator.Play(animation);
    }

    public bool IsColliderAbove
    {
        get
        {
            var raycast = Physics2D.RaycastAll(collider2D.bounds.center + new Vector3(0, collider2D.bounds.extents.y, 0.01f), Vector2.up, 0.7f, platformLayer);
            foreach (var cast in raycast)
            {
                if (cast.collider.gameObject != gameObject && !cast.collider.isTrigger)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    public bool IsDead { get => Health <= 0; }

    public bool IsOnLadder = false;
}

public interface OnAttack
{
    void Attack();

    bool ChargeAttack { get; set; }
}

public interface OnDamage
{
    void Damage(int damage, GameObject who);
}

public class DefaultDamageController : OnDamage
{
    private Character character;

    public DefaultDamageController(Character character)
    {
        this.character = character;
    }

    public void Damage(int damage, GameObject who)
    {
        if (character.isTeleporting)
            return;

        if (character == null && character.Health > 0)
            return;

        if (character.OnDamage != null)
            character.OnDamage.Invoke();

        character.Health -= damage;
        if (character.Health <= 0)
        {
            character.gameObject.layer = LayerMask.NameToLayer("Dead");
            if (character.OnDeath != null)
                character.OnDeath.Invoke();
        }
    }
}