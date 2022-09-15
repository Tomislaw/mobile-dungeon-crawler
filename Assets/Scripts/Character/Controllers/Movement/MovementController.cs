
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;


namespace RuinsRaiders
{
    [RequireComponent(typeof(Character))]
    public class MovementController : MonoBehaviour
    {
        public bool flying = false;

        // movement properties
        public float gravityScale = 6;

        public float climbLadderSpeed = 8;


        public float coyoteTime = 0.15f;
        public float walkSpeed = 10;
        public float swimSpeed = 8;
        public float jumpSpeed = 20;
        public float maxSpeed = 30;
        public float acceleration = 10;
        public float jerk = 10;
        public bool canUseLadder = true;
        public bool canSwim = true;

        public float stepTime = 0.15f;

        public UnityEvent onJump;
        public UnityEvent onWalk;
        public UnityEvent onSwim;

        // on which platforms or ladders character currently is
        public HashSet<LadderTile> ladders = new ();
        public HashSet<WaterTile> waters = new();
        public HashSet<PlatformTile> platforms = new ();
        public Vector2 move = new ();

        private Vector2 _previousMove = new();
        private bool _wasSwimming;

        private Character _character;
        private Rigidbody2D _rigidbody;
        private Collider2D _collider2D;

        private float _timeToFinishCoyoteTime = 0;
        private float _timeToStep = 0;

        private float _accumulatedAcceleration;

        public bool FaceLeft { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsMoving { get => move.x != 0; }
        public bool IsOnLadder { get; private set; }

        public bool IsSwimming { get => waters.Count > 0; }


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

        public bool IsLanded { get; private set; }

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
            FaceLeft = false;

            if(canUseLadder && climbLadderSpeed <= 0)
                Debug.LogErrorFormat("{0} can climb but climb speed os set to {1}!", name, climbLadderSpeed);

            if (canSwim && swimSpeed <= 0)
                Debug.LogErrorFormat("{0} can swim but climb speed os set to {1}!", name, swimSpeed);

            if (flying && gravityScale > 0)
                Debug.LogErrorFormat("{0} is flying but gravity scale is set to {1}!", name, gravityScale);

            if(walkSpeed > maxSpeed || swimSpeed > maxSpeed || jumpSpeed > maxSpeed)
                Debug.LogErrorFormat("{0} movement speed is higher than max speed of {1}!", name, maxSpeed);
        }

        private bool CheckForGround()
        {
            //if (System.Math.Round(rigidbody.velocity.y, 1) != 0)
            //    return false;
            var raycast = Physics2D.BoxCastAll(transform.position + new Vector3(0, 0.1f, 0),
                  new Vector3(_collider2D.bounds.size.x - 0.02f, 0.01f), 0, Vector2.down, 0.2f);
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
            if (_character.holdUpdate)
                return;

            if (_character.IsDead)
                Stop();

            var landed = IsGrounded;
            IsGrounded = CheckForGround();
            IsLanded = !landed && IsGrounded;

            HandleDirection();

            if (flying || IsSwimming && canSwim)
                FlyingMovement();
            else
                WalkingMovement();

            _previousMove = move;

            // clamp to max speed
            if (_rigidbody.velocity.magnitude > maxSpeed)
                _rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed;
        }

        private void HandleDirection()
        {
            // set direction based on movement
            if (move.x > 0)
                FaceLeft = false;

            if (move.x < 0)
                FaceLeft = true;

            // change scale based on face direction
            if (FaceLeft)
                transform.localScale = new Vector2(-1, 1);
            else
                transform.localScale = new Vector2(1, 1);
        }

        private void WalkingMovement()
        {
            if (move.x == 0 || Mathf.Sign(_previousMove.x) != Mathf.Sign(move.x))
                _accumulatedAcceleration = acceleration;
            else
                _accumulatedAcceleration += jerk * Time.fixedDeltaTime;

            if (ladders.Count == 0 && canUseLadder)
                IsOnLadder = false;

            if (move.y != 0 && ladders.Count > 0 && canUseLadder)
                IsOnLadder = true;

            if (IsGrounded)
            {
                IsJumping = false;
                _timeToFinishCoyoteTime = coyoteTime;
            }
            else
                _timeToFinishCoyoteTime -= Time.fixedDeltaTime;

            // move on ground and slopes
            var slope = GetSlopeFactor();
            if (IsLanded)
            {
                // todo: special behavior on landing on slopes
            }
            bool onSlope = slope.x < 0.99f;

            bool canJump = (
                (IsGrounded || _timeToFinishCoyoteTime > 0)
                && !IsJumping
                && !IsColliderAbove
                && (onSlope || _rigidbody.velocity.y <= 0.1f)
                )
                || _wasSwimming && !IsSwimming && !IsJumping;


            if (IsOnLadder && canUseLadder)
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, climbLadderSpeed * move.y);
            }
            else if (canJump && move.y > 0)
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpSpeed * move.y);
                IsJumping = true;
                onJump.Invoke();
            }

            // get move direction x
            int moveDir = 0;
            if (_rigidbody.velocity.x > walkSpeed * move.x && move.x <= 0)
                moveDir = -1;
            else if (_rigidbody.velocity.x < walkSpeed * move.x && move.x >= 0)
                moveDir = 1;


            if (moveDir != 0)
            {
                float speed = _accumulatedAcceleration * Time.fixedDeltaTime * moveDir;
                Vector2 additionalSpeed = new(speed * slope.x, speed * slope.y);
                AddSpeed(additionalSpeed, walkSpeed * Mathf.Abs(move.x));
            }

            if (onSlope || IsOnLadder)
                _rigidbody.gravityScale = 0;
            else
                _rigidbody.gravityScale = gravityScale;


            if (IsGrounded && move.x != 0)
            {
                _timeToStep -= Time.fixedDeltaTime;
                if (_timeToStep < 0)
                {
                    _timeToStep = stepTime;
                    onWalk.Invoke();
                }
            }
        }

        private void FlyingMovement()
        {
            if (move.x == 0 || Mathf.Sign(_previousMove.x) != Mathf.Sign(move.x))
                _accumulatedAcceleration = acceleration;
            else
                _accumulatedAcceleration += jerk * Time.fixedDeltaTime;

            if (IsSwimming)
                _rigidbody.gravityScale = 0;
            else
                _rigidbody.gravityScale = gravityScale;

            if (_rigidbody.velocity.y > walkSpeed * move.y && move.y <= 0)
            {
                float additionalSpeed = _accumulatedAcceleration * Time.fixedDeltaTime;
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Max(
                    (IsSwimming ? swimSpeed : walkSpeed) * move.y, _rigidbody.velocity.y - additionalSpeed));
            }

            if (_rigidbody.velocity.y < walkSpeed * move.y && move.y >= 0)
            {
                float additionalSpeed = _accumulatedAcceleration * Time.fixedDeltaTime;
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Min(
                    (IsSwimming ? swimSpeed : walkSpeed) * move.y, _rigidbody.velocity.y + additionalSpeed));
            }

            int moveDir = 0;
            if (_rigidbody.velocity.x > walkSpeed * move.x && move.x <= 0)
                moveDir = -1;
            else if (_rigidbody.velocity.x < walkSpeed * move.x && move.x >= 0)
                moveDir = 1;


            var slope = GetSlopeFactor();


            if (moveDir != 0)
            {
                float speed = _accumulatedAcceleration * Time.fixedDeltaTime * moveDir;
                Vector2 additionalSpeed = new(speed * slope.x, speed * slope.y);
                AddSpeed(additionalSpeed, (IsSwimming ? swimSpeed : walkSpeed) * Mathf.Abs(move.x));
            }

            _wasSwimming = IsSwimming && canSwim;
            IsJumping = false;
        }

        private Vector2 GetSlopeFactor()
        {

            if (IsJumping)
                return new Vector2(1, 0);

            var center = _collider2D.bounds.center;
            var height = _collider2D.bounds.size.y;

            var hit1 = Physics2D.Raycast(center, Vector2.down, height, Physics2D.GetLayerCollisionMask(_collider2D.gameObject.layer));

            float dir = FaceLeft ? -1 : 1;
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
            Vector2 newSpeed = _rigidbody.velocity;
            if (addSpeed.x < 0 && _rigidbody.velocity.x > limitMin.x)
            {
                newSpeed.x += addSpeed.x;
                if (newSpeed.x < limitMin.x)
                    newSpeed.x = limitMin.x;
            }
            if (addSpeed.x > 0 && _rigidbody.velocity.x < limitMax.x)
            {
                newSpeed.x += addSpeed.x;
                if (newSpeed.x > limitMax.x)
                    newSpeed.x = limitMax.x;
            }
            if (addSpeed.y < 0 && _rigidbody.velocity.y > limitMin.y)
            {
                newSpeed.y += addSpeed.y;
                if (newSpeed.y < limitMin.y)
                    newSpeed.y = limitMin.y;
            }
            if (addSpeed.y > 0 && _rigidbody.velocity.y < limitMax.y)
            {
                newSpeed.y += addSpeed.y;
                if (newSpeed.y > limitMax.y)
                    newSpeed.y = limitMax.y;
            }
            _rigidbody.velocity = newSpeed;
        }

        public void Teleport(Vector2 position)
        {
            StartCoroutine(TeleportCoroutine(position));
        }

        private IEnumerator TeleportCoroutine(Vector2 position)
        {
            if (_rigidbody)
                _rigidbody.velocity = new Vector2();
            _character.holdUpdate = true;
            transform.position = position;
            _character.SetAnimation("Dead");
            yield return new WaitForSeconds(0.3f);
            _character.SetAnimation("Idle");
            _character.holdUpdate = false;
        }

        public void FacePosition(Vector2 position)
        {
            FaceLeft = position.x < transform.position.x;
        }
    }
}

