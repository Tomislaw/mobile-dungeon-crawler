﻿using System.Collections;
using UnityEngine;


namespace RuinsRaiders
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Character : MonoBehaviour
    {
        [HideInInspector]
        public bool holdUpdate;

        [SerializeField]
        private bool setAnimationVariablesInstead;

        // fields used for animations
        [SerializeField]
        private bool haveSneakAnimation = false;
        [SerializeField]
        private bool haveWalkPreAttackAnimation = false;
        [SerializeField]
        private bool haveOverchargeAnimation = false;
        [SerializeField]
        private bool haveOverchargeAttackAnimation = false;
        [SerializeField]
        private bool haveJumpAnimation = false;

        private string _currentAnimation;

        public HealthController.Group Group { get => HealthController.group; }

        public MovementController MovementController { get; private set; }
        public AttackController AttackController { get; private set; }
        public HealthController HealthController { get; private set; }
        // events

        private Animator _animator;


        private void OnEnable()
        {

            _animator = GetComponent<Animator>();

            AttackController = GetComponent<AttackController>();
            MovementController = GetComponent<MovementController>();
            HealthController = GetComponent<HealthController>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (holdUpdate)
                return;

            if (setAnimationVariablesInstead)
            {
                _animator.SetBool("IsDead", IsDead);
                _animator.SetBool("IsChargingAttack", AttackController.chargeAttack);
                _animator.SetBool("IsOvercharged", AttackController.IsOvercharged);
                _animator.SetBool("IsAttacking", AttackController.IsAttacking);
                _animator.SetBool("IsJumpingStarted", MovementController.IsJumpingStarted);
                _animator.SetBool("IsJumping", MovementController.IsJumping);
                _animator.SetBool("IsLanded", MovementController.IsLanded);
                _animator.SetBool("IsMoving", MovementController.IsMoving);
                _animator.SetBool("IsSwimming", MovementController.IsSwimming);
                _animator.SetBool("IsColliderAbove", MovementController.IsColliderAbove);
                _animator.SetBool("IsGrounded", MovementController.IsGrounded);
                _animator.SetFloat("VelocityX", MovementController.Velocity.x);
                _animator.SetFloat("VelocityY", MovementController.Velocity.y);
                _animator.SetBool("IsOnLadder", MovementController.IsOnLadder);
                return;
            }

            string suffix = haveSneakAnimation && (MovementController.IsColliderAbove || MovementController.IsSwimming) ? "Sneak" : "";
            if (IsDead)
                SetAnimation("Dead");
            else if (AttackController.chargeAttack && AttackController.CanAttack)
            {
                if (haveWalkPreAttackAnimation && MovementController.IsMoving)
                    suffix = "Walk" + suffix;

                if (haveOverchargeAnimation && AttackController.IsOvercharged)
                    suffix = "2" + suffix;

                SetAnimation("PreAttack" + suffix);

            }
            else if (AttackController.IsAttacking)
            {
                if (haveOverchargeAttackAnimation && AttackController.IsOvercharged)
                    SetAnimation("Attack2" + suffix);
                else
                    SetAnimation("Attack" + suffix);
            }
            else if (haveJumpAnimation && MovementController.IsJumping && MovementController.Velocity.y > 0)
                SetAnimation("Jump");
            else if (haveJumpAnimation && MovementController.IsJumping && MovementController.Velocity.y < 0)
                SetAnimation("Fall");
            else if (MovementController.IsMoving)
                SetAnimation("Walk" + suffix);
            else if (IsDead)
                SetAnimation("Dead");
            else
                SetAnimation("Idle" + suffix);

        }

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
            if (_currentAnimation == animation)
                return;
            _currentAnimation = animation;
            _animator.Play(animation);

        }

        public bool IsDead { get => HealthController ? HealthController.IsDead : false; }

    }
}