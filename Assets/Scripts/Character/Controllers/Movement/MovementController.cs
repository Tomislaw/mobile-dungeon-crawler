
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using System.Linq;

namespace RuinsRaiders
{

    [RequireComponent(typeof(Character))]
    public class MovementController : MonoBehaviour
    {
        [Header("Controlls")]
        public Vector2 move = new();
        public bool faceLeft;
        public float jumpingTreshold = 0f;

        [Header("Type")]
        public bool flying = false;
        public float gravityScale = 6;
        public float waterGravityScale = 6;

        [Header("Jump")]
        public float coyoteTime = 0.15f;
        public float jumpSpeed = 20;
        public float jumpCutGravityMultiplier = 2f;
        public float jumpHangTimeTreshold = 5;
        public float jumpHangSpeedGravityMultiplier = 0.5f;


        [Header("Movement")]
        public float walkSpeed = 10;
        public float swimSpeed = 8;
        public float climbSpeed = 8;

        public float acceleration = 10;
        public float deacceleration = 10;

        public float maxSpeed = 30;


        [Header("Flags")]
        public bool canUseLadder = true;
        public bool canSwim = true;
        public bool canUsePlatform = true;

        public float stepTime = 0.15f;

        [Header("Events")]
        public UnityEvent onJump;
        public UnityEvent onWalk;
        public UnityEvent onSwim;
        public UnityEvent onLand;

        public HashSet<LadderTile> ladders = new();
        public HashSet<WaterTile> waters = new();
        public HashSet<PlatformTile> platforms = new();

        private Vector2 _previousMove = new();
        private bool _wasSwimming;
        private bool _jumpCut;

        private float _timeToFinishCoyoteTime = 0;
        private float _timeToStep = 0;

        private Character _character;
        private Rigidbody2D _rigidbody;
        private Collider2D _collider2D;


        private float MovementSpeed { get => IsSwimming ? swimSpeed : walkSpeed; }

        public bool IsJumping { get; private set; }
        public bool IsMoving { get => move.x != 0; }
        public bool IsOnLadder { get; private set; }

        public bool IsSwimming { get; private set; }

        public bool IsInWater { get => waters.Count > 0; }

        public Vector2 Velocity { get => _rigidbody.velocity; }

        public float Mass { get => _rigidbody.mass; }

        public bool IsColliderAbove { get; private set; }

        public bool IsGrounded { get; private set; }

        public bool IsJumpingStarted { get; private set; }

        public bool IsLanded { get; private set; }

        public bool OnSlope { get; private set; }

        public Vector2 Slope { get; private set; }

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
            _character = GetComponent<Character>();
            _collider2D = GetComponent<Collider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();

            if (canUseLadder && climbSpeed <= 0)
                Debug.LogErrorFormat("{0} can climb but climb speed os set to {1}!", name, climbSpeed);

            if (canSwim && swimSpeed <= 0)
                Debug.LogErrorFormat("{0} can swim but climb speed os set to {1}!", name, swimSpeed);

            if (walkSpeed > maxSpeed || swimSpeed > maxSpeed || jumpSpeed > maxSpeed)
                Debug.LogErrorFormat("{0} movement speed is higher than max speed of {1}!", name, maxSpeed);
        }

        private bool CheckForGround()
        {
            var legsPosition = _collider2D.bounds.center - new Vector3(0, _collider2D.bounds.size.y/2f);
            var raycast = Physics2D.BoxCastAll(legsPosition + new Vector3(0, 0.1f, 0),
                  new Vector3(_collider2D.bounds.size.x - 0.01f, 0.01f), 0, Vector2.down, 0.2f);
            foreach (var cast in raycast)
            {
                if (cast.collider.gameObject != gameObject
                    && !cast.collider.isTrigger
                    && !Physics2D.GetIgnoreLayerCollision(cast.collider.gameObject.layer, gameObject.layer))
                    return true;
            }
            return false;
        }

        private bool CheckForCeiling()
        {
            var headPosition = _collider2D.bounds.center + new Vector3(0, _collider2D.bounds.size.y / 2f);
            var raycast = Physics2D.BoxCastAll(headPosition + new Vector3(0, -0.1f, 0),
                  new Vector3(_collider2D.bounds.size.x, 0.01f), 0, Vector2.up, 0.2f);
            foreach (var cast in raycast)
            {
                if (cast.collider is TilemapCollider2D || cast.collider is CompositeCollider2D)
                    return true;
            }
            return false;
        }

        public void Update()
        {

            if (IsGrounded && move.x != 0)
            {
                _timeToStep -= Time.deltaTime;
                if (_timeToStep < 0)
                {
                    _timeToStep = stepTime;
                    onWalk.Invoke();
                }
            }

            if (IsSwimming && (move != new Vector2()))
            {
                _timeToStep -= Time.deltaTime;
                if (_timeToStep < 0)
                {
                    _timeToStep = stepTime;
                    onSwim.Invoke();
                }
            }
        }

        public void FixedUpdate()
        {
            if (_character.holdUpdate)
                return;

            if (_character.IsDead)
                Stop();

            if(_timeToFinishCoyoteTime > 0)
                _timeToFinishCoyoteTime -= Time.fixedDeltaTime;

            GroundCheck();
            DirectionCheck();
            FlagsCheck();


            if (_character.IsDead)
                DeadMovement();
            else if (IsJumping)
                JumpingMovement();
            else if (flying || IsSwimming)
                FlyingMovement();
            else if (IsOnLadder)
                LadderMovement();
            else
                WalkingMovement();

            if (move.y > jumpingTreshold && CanJump())
                Jump();

            _previousMove = move;

            _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, maxSpeed);
        }

        private void GroundCheck()
        {
            var wasGrounded = IsGrounded;
            IsGrounded = CheckForGround();
            IsLanded = !wasGrounded && IsGrounded && _rigidbody.velocity.y < 0.001f;
            if(IsLanded)
                onLand.Invoke();

            if (IsGrounded && _rigidbody.velocity.y < 0.001f)
                IsJumping = false;

            IsColliderAbove = CheckForCeiling();
            Slope = GetSlopeFactor();
            OnSlope = Slope.x < 0.99f;

            if(IsGrounded)
                _timeToFinishCoyoteTime = coyoteTime;
        }

        private void DirectionCheck()
        {
            if (move.x > 0)
                faceLeft = false;

            if (move.x < 0)
                faceLeft = true;

            if (faceLeft)
                transform.localScale = new Vector2(-1, 1);
            else
                transform.localScale = new Vector2(1, 1);
        }


        private void FlagsCheck()
        {
            IsJumpingStarted = false;

            if (canUseLadder)
            {
                if (ladders.Count == 0)
                {
                    IsOnLadder = false;
                }
                else if (IsOnLadder == false && move.y != 0 && ladders.Count > 0)
                {

                    _rigidbody.velocity = new Vector2();
                    IsOnLadder = true;
                    IsJumping = false;
                }
            }
            if (canSwim)
            {
                _wasSwimming = IsSwimming;
                IsSwimming = IsInWater;
                if(IsSwimming)
                    IsJumping = false;
            }
        }

        private Vector2 MovementAdjust(Vector2 movement)
        {
            if(movement.y > 0 && IsColliderAbove)
            {
                var topLeft = _collider2D.bounds.center + new Vector3(-_collider2D.bounds.size.x / 2f, _collider2D.bounds.size.y / 2f - 0.1f);
                var topRight = _collider2D.bounds.center + new Vector3(_collider2D.bounds.size.x / 2f, _collider2D.bounds.size.y / 2f - 0.1f);

                var hitLeft = Physics2D.Raycast(topLeft, Vector2.up, 0.5f, Physics2D.GetLayerCollisionMask(_collider2D.gameObject.layer));
                var hitRight = Physics2D.Raycast(topRight, Vector2.up, 0.5f, Physics2D.GetLayerCollisionMask(_collider2D.gameObject.layer));

                if (hitLeft && hitRight)
                    return movement;

                if(hitLeft)
                    return movement + new Vector2(0.2f, 0);

                if (hitRight)
                    return movement + new Vector2(-0.2f, 0);
            }

            if(movement.y < 0 && IsGrounded)
            {
                var bottomLeft = _collider2D.bounds.center + new Vector3(-_collider2D.bounds.size.x / 2f, -_collider2D.bounds.size.y / 2f + 0.1f);
                var bottomRight = _collider2D.bounds.center + new Vector3(_collider2D.bounds.size.x / 2f, -_collider2D.bounds.size.y / 2f + 0.1f);

                var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.5f, Physics2D.GetLayerCollisionMask(_collider2D.gameObject.layer));
                var hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.5f, Physics2D.GetLayerCollisionMask(_collider2D.gameObject.layer));

                if (hitLeft && hitRight)
                    return movement;

                if (hitLeft)
                    return movement + new Vector2(0.2f, 0);

                if (hitRight)
                    return movement +  new Vector2(-0.2f, 0);
            }

            if(IsOnLadder && Mathf.Abs(move.x) < 0.2f && Mathf.Abs(move.y) > 0.2f)
            {
                var ladder = ladders.OrderBy(it => Vector2.Distance(transform.position, it.transform.position)).First();
                var diff = ladder.transform.position.x - transform.position.x;
                return movement + new Vector2(Mathf.Clamp(diff, -0.8f, 0.8f), 0);
            }

            return movement;
        }

        private void LadderMovement()
        {
            SetGravityScale(0);
            Vector2 targetSpeed = MovementAdjust(move) * climbSpeed;
            Vector2 speedDif = targetSpeed - _rigidbody.velocity;

            float accelRateX = (Mathf.Abs(targetSpeed.x) > 0.01f) ? acceleration : deacceleration;
            float accelRateY = (Mathf.Abs(targetSpeed.y) > 0.01f) ? acceleration : deacceleration;

            Vector2 movement;
            movement.x = speedDif.x * accelRateX;
            movement.y = speedDif.y * accelRateY;

            _rigidbody.AddForce(movement, ForceMode2D.Force);
        }

        private bool CanJump()
        {
            return (IsGrounded || _timeToFinishCoyoteTime > 0)
                && !IsJumping
                && !IsColliderAbove
                && !IsLanded
                && !flying
                && !_character.IsDead
                && !IsSwimming
                && !IsOnLadder
                || _wasSwimming && !IsSwimming && !IsJumping;
        }

        private void SetGravityScale(float gravity)
        {
            _rigidbody.gravityScale = gravity;
        }

        private void JumpingMovement()
        {
            _jumpCut = move.y <= 0;

            var scale = IsInWater ? waterGravityScale : gravityScale;
            if (_jumpCut && _rigidbody.velocity.y > 0)
                SetGravityScale(scale * jumpCutGravityMultiplier);
            else if (Mathf.Abs(_rigidbody.velocity.y) < jumpHangTimeTreshold)
                SetGravityScale(scale * jumpHangSpeedGravityMultiplier);
            else
                SetGravityScale(scale);

            var speed = IsInWater ? swimSpeed : walkSpeed;
            float targetSpeed = move.x * speed;

            // acceleration
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deacceleration;

            float speedDif = targetSpeed - _rigidbody.velocity.x;
            float movement = speedDif * accelRate;

            _rigidbody.AddForce(Vector2.right * movement, ForceMode2D.Force);

        }

        private void WalkingMovement()
        {
            var scale = IsInWater ? waterGravityScale : gravityScale;
            SetGravityScale(IsLanded || OnSlope ? 0f : scale);

            var speed = IsInWater ? swimSpeed : walkSpeed;
            float targetSpeedMagnitude = move.x * speed;
            float accelRate = (Mathf.Abs(targetSpeedMagnitude) > 0.01f) ? acceleration : deacceleration;

            if (OnSlope)
            {
                Vector2 targetSpeed = new(targetSpeedMagnitude * Slope.x, targetSpeedMagnitude * Slope.y);
                Vector2 speedDif = targetSpeed - _rigidbody.velocity;
                Vector2 movement = speedDif * accelRate;
                _rigidbody.AddForce(movement, ForceMode2D.Force);
            }
            else
            {
                float speedDif = targetSpeedMagnitude - _rigidbody.velocity.x;
                float movement = speedDif * accelRate;
                _rigidbody.AddForce(Vector2.right * movement, ForceMode2D.Force);
            }

        }

        private void Jump()
        {
            IsJumping = true;
            IsJumpingStarted = true;
            waters.Clear();
            _rigidbody.velocity = new(_rigidbody.velocity.x, jumpSpeed);
            onJump.Invoke();
        }

        private void FlyingMovement()
        {
            var scale = IsInWater ? waterGravityScale : gravityScale;
            SetGravityScale(IsLanded || OnSlope ? 0f : scale);

            var speed = IsInWater ? swimSpeed : walkSpeed;
            Vector2 targetSpeed = MovementAdjust(move) * speed;
            Vector2 speedDif = targetSpeed - _rigidbody.velocity;

            float accelRateX = (Mathf.Abs(targetSpeed.x) > 0.01f) ? acceleration : deacceleration;
            float accelRateY = (Mathf.Abs(targetSpeed.y) > 0.01f) ? acceleration : deacceleration;

            Vector2 movement;
            movement.x = speedDif.x * accelRateX;
            movement.y = speedDif.y * accelRateY;

            _rigidbody.AddForce(movement, ForceMode2D.Force);
        }

        private void DeadMovement()
        {
            if(IsInWater)
                SetGravityScale(0.5f);
            else
                SetGravityScale(6);
            if (IsGrounded) { 
                _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
                _rigidbody.drag = 20;
            }
            else
            {
                _rigidbody.drag = 2;
            }
        }

        private Vector2 GetSlopeFactor()
        {

            if (IsJumping || !IsGrounded)
                return new Vector2(1, 0);

            var center = _collider2D.bounds.center;
            var height = _collider2D.bounds.size.y;

            var hit1 = Physics2D.Raycast(center, Vector2.down, height, Physics2D.GetLayerCollisionMask(_collider2D.gameObject.layer));

            float dir = faceLeft ? -1 : 1;
            center.x += _collider2D.bounds.size.x * dir / 2f;
            center.y -= 0.001f;

            var hit2 = Physics2D.Raycast(center, Vector2.down, height, Physics2D.GetLayerCollisionMask(_collider2D.gameObject.layer));

            if (hit1 && hit2)
            {
                return new Vector2(Mathf.Max(hit1.normal.y, hit2.normal.y), -Mathf.Min(hit1.normal.x * dir, hit2.normal.x * dir) * dir);
            }
            else if (hit1)
                return new Vector2(hit1.normal.y, -hit1.normal.x);
            else if (hit2)
                return new Vector2(hit2.normal.y, -hit2.normal.x);

            return new Vector2(1, 0);
        }

        public void Teleport(Vector2 position)
        {
            _character.holdUpdate = true;
            StartCoroutine(TeleportCoroutine(position));
        }

        private IEnumerator TeleportCoroutine(Vector2 position)
        {
            if (_rigidbody)
                _rigidbody.velocity = new Vector2();

            transform.position = position;
            _character.SetAnimation("Dead");
            yield return new WaitForSeconds(0.3f);
            _character.SetAnimation("Idle");
            _character.holdUpdate = false;
        }

        public void FacePosition(Vector2 position)
        {
            faceLeft = position.x < transform.position.x;
        }
    }
}

