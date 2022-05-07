using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{

    // fields used for animations
    public bool HaveSneakAnimation = false;
    public bool HaveWalkPreAttackAnimation = false;
    public bool HaveOverchargeAnimation = false;
    public HealthController.Group Group { get => HealthController.group; }

    public MovementController MovementController { get; private set; }
    public AttackController AttackController { get; private set; }
    public HealthController HealthController { get; private set; }
    // events

    private Animator animator;



    private void OnEnable()
    {

        animator = GetComponent<Animator>();

        AttackController = GetComponent<AttackController>();
        MovementController = GetComponent<MovementController>();
        HealthController = GetComponent<HealthController>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (holdUpdate)
            return;

        string suffix = HaveSneakAnimation && MovementController.IsColliderAbove ? "Sneak" : "";
        if (IsDead)
        {
            SetAnimation("Dead");
            
        }
        else if(AttackController.ChargeAttack && AttackController.CanAttack)
        {
            if (HaveWalkPreAttackAnimation && MovementController.IsMoving)
                suffix = "Walk" + suffix;

            if (HaveOverchargeAnimation && AttackController.IsOvercharged)
                suffix = "2" + suffix;

            SetAnimation("PreAttack" + suffix);
            
        }
        else if (AttackController.IsAttacking)
        {
            SetAnimation("Attack" + suffix);
        }
        else if (MovementController.IsMoving)
        {
            SetAnimation("Walk" + suffix);
        }
        else if (IsDead)
        {
            SetAnimation("Dead");
        }
         
        else
            SetAnimation("Idle" + suffix);


    }



    private string currentAnimation;

    public bool holdUpdate = false;



    public void Hide()
    {
        StartCoroutine(HideCoroutine());
    }

    public IEnumerator HideCoroutine()
    {
        holdUpdate = true;
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

    public bool IsDead { get => HealthController ? HealthController.IsDead : false; }

  

}