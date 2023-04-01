using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{

    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(Rigidbody2D))]
    public class RollableBox : MonoBehaviour
    {

        public UnityEvent OnRoll = new UnityEvent();
        public UnityEvent OnSlopeRoll = new UnityEvent();
        public UnityEvent OnFall = new UnityEvent();

        [SerializeField]
        private float timeToRoll = 0.25f;
        [SerializeField]
        private float timeToPush = 0.2f;
        [SerializeField]
        private float timeToFall = 0.2f;

        [SerializeField]
        [Range(0f, 1f)]
        private float accelerationPercentage = 0.2f;

        [SerializeField]
        private int rotations = 6;
        [SerializeField]
        private int fallPoints = 3;

        private float _timeLeftToPush = 0;

        private List<MovementController> _pushers = new();
        private AStar _astar;
        private Collider2D _collider;
        private Rigidbody2D _rigidbody;
        private HealthController _healthController;

        internal int _accumulatedRoll = 0;

        public bool IsDead { get => _healthController != null && _healthController.IsDead; }

        public float Rotation { get; private set; }

        public bool IsRolling { get; private set; }
        public bool CanRollLeft
        {
            get => !IsFalling && !IsRolling && _astar != null && CanRoll(true, _astar.GetTileId(transform.position));
        }
        public bool CanRollRight
        {
            get => !IsFalling && !IsRolling && _astar != null && CanRoll(false, _astar.GetTileId(transform.position));
        }

        private bool CanRoll(bool left, Vector2Int id)
        {
            if (_astar == null)
                return false;

            var sign = left ? -1 : 1;
            var tile = _astar.GetTileData(id + new Vector2Int(sign, 0));
            return tile.Block == false && !tile.Slope;
        }


        public bool IsFalling { get; private set; }

        private Coroutine coroutine;


        private void Start()
        {
            _astar = FindObjectOfType<AStar>();
            Rotation = transform.rotation.eulerAngles.z;
            _collider = GetComponent<Collider2D>();
            _healthController = GetComponent<HealthController>();
            _rigidbody = GetComponent<Rigidbody2D>();

            var health = GetComponent<HealthController>();

            if (!IsDead)
                _astar.AddDynamicBlockTile(gameObject);

            if (health != null)
                health.onDeath.AddListener(UpdateStandings);
            if (_astar == null)
                return;
            _astar.onMapUpdated.AddListener(UpdateStandings);
        }


        private void OnEnable()
        {
            if (_astar == null || _collider == null || _collider.isActiveAndEnabled)
                return;

            if (IsDead)
                _astar.RemoveDynamicBlockTile(gameObject);
            else
                _astar.AddDynamicBlockTile(gameObject);
        }

        private void OnDisable()
        {
            if (_astar == null)
                return;

            _astar.RemoveDynamicBlockTile(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var pusher = collision.gameObject.GetComponent<MovementController>();
            if (pusher)
            {
                _pushers.Add(pusher);
                _timeLeftToPush = timeToPush;
            }

        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var pusher = collision.gameObject.GetComponent<MovementController>();
            if (pusher)
            {
                _pushers.Remove(pusher);
                _timeLeftToPush = timeToPush;
            }
        }

        private void FixedUpdate()
        {
            if (coroutine != null)
                return;

            if (_pushers.Count == 0 || IsDead)
                return;

            var pusher = _pushers
                .Aggregate((agg, next) =>
                next.Mass > agg.Mass ? next : agg);

            // cannot push when standing on it
            if (pusher.transform.position.y > transform.position.y + 0.48)
            {
                _timeLeftToPush = timeToPush;
                return;
            }

            // cannot push when not moving
            if (!pusher.IsMoving)
            {
                _timeLeftToPush = timeToPush;
                return;
            }

            _timeLeftToPush -= Time.fixedDeltaTime;
            if (_timeLeftToPush < 0)
            {
                if (pusher.faceLeft && !CanRollLeft)
                    return;
                if (!pusher.faceLeft && !CanRollRight)
                    return;

                _accumulatedRoll = 0;
                coroutine = StartCoroutine(Roll(pusher.faceLeft));
            }

        }

        public void UpdateStandings()
        {
            if (!isActiveAndEnabled)
                return;

            if (coroutine != null)
                return;

            if (IsDead && _collider != null && _collider.enabled == true)
            {
                _collider.enabled = false;
                ResetRotation();
                _astar.RemoveDynamicBlockTile(gameObject);
            }

            SnapToGrid();

            if (_astar == null)
                return;

            var id = _astar.GetTileId(transform.position);
            var bottomId = id + new Vector2Int(0, -1);
            var bottomNode = _astar.GetTileData(bottomId);
            if (bottomNode.Slope && !IsDead)
            {
                if (CanRoll(true, bottomId))
                {
                    _accumulatedRoll += -1;
                    coroutine = StartCoroutine(RollOnSlope(true));
                }
                else if (CanRoll(false, bottomId))
                {
                    _accumulatedRoll += 1;
                    coroutine = StartCoroutine(RollOnSlope(false));
                }
            }
            else if (!bottomNode.Block && !bottomNode.Platform)
            {
                if (_accumulatedRoll < 0 && CanRoll(true, bottomId))
                {
                    _accumulatedRoll = 0;
                    coroutine = StartCoroutine(RollOnSlope(true));
                }
                else if (_accumulatedRoll > 0 && CanRoll(false, bottomId))
                {
                    _accumulatedRoll = 0;
                    coroutine = StartCoroutine(RollOnSlope(false));
                }
                else
                {
                    _accumulatedRoll = 0;
                    coroutine = StartCoroutine(Fall());
                }
            }
            else if (_accumulatedRoll != 0)
            {
                if (_accumulatedRoll > 0 && CanRoll(false, id))
                {
                    _accumulatedRoll -= 1;
                    coroutine = StartCoroutine(Roll(false));
                }
                else if (_accumulatedRoll < 0 && CanRoll(true, id))
                {
                    _accumulatedRoll += 1;
                    coroutine = StartCoroutine(Roll(true));
                } else
                {
                    _accumulatedRoll = 0;
                }
            }
        }

        public void SnapToGrid()
        {
            _rigidbody.MovePosition(new Vector3(
                (float)Math.Round(transform.position.x * 2, MidpointRounding.AwayFromZero) / 2,
                (float)Math.Round(transform.position.y * 2, MidpointRounding.AwayFromZero) / 2,
                transform.position.z));
        }

        private IEnumerator Fall()
        {
            if (IsFalling == true || IsRolling == true)
                yield break;

            IsFalling = true;

            if (!IsDead)
            {
                yield return new WaitForFixedUpdate();
                _astar.RemoveDynamicBlockTile(gameObject);
            }

            int falledTiles = 0;
            var movement = 1f / fallPoints;

            var acceleration = 1f;
            var accelearationFactor = 1f - accelerationPercentage;

            while (IsFalling)
            {

                var startingPosition = transform.position;
                for (int i = 0; i < fallPoints; i++)
                {
                    _rigidbody.MovePosition(transform.position - new Vector3(0, movement, 0));
                    yield return new WaitForSeconds((timeToFall / fallPoints) * acceleration);
                }
                transform.position = startingPosition + new Vector3(0, -1, 0);

                if (OnFall != null)
                    OnFall.Invoke();

                    // stop if reached block or platform
                var bottomId = _astar.GetTileId(transform.position) + new Vector2Int(0, -1);
                var bottomTile = _astar.GetTileData(bottomId);

                if (bottomTile.Block || bottomTile.Platform || bottomTile.Slope)
                {
                    IsFalling = false;
                    break;
                }

                acceleration *= accelearationFactor;
                falledTiles++;
            }

            if (!IsDead)
            {
                _astar.AddDynamicBlockTile(gameObject);
            }

            yield return new WaitForSeconds((timeToFall / (float)fallPoints));


            coroutine = null;
            UpdateStandings();
        }
        private IEnumerator Roll(bool left)
        {
            if (!isActiveAndEnabled)
                yield break;

            if (_astar == null)
                yield break;

            if (IsFalling == true || IsRolling == true)
                yield break;

            IsRolling = true;
            IsFalling = false;

            if (!IsDead)
            {
                _astar.RemoveDynamicBlockTile(gameObject);
            }

            var sign = left ? 1 : -1;
            var step = sign * 90f / (float)rotations;
            var movement = -1 * sign * 1f / (float)rotations;
            var startingPosition = transform.position;
            for (int i = 0; i < rotations; i++)
            {
                Rotation += step;
                _rigidbody.MovePosition(transform.position + new Vector3(movement, 0, 0));
                _rigidbody.SetRotation(Rotation);
                yield return new WaitForSeconds(timeToRoll / (float)rotations);

            }
            transform.position = startingPosition + new Vector3(-sign, 0, 0);

            SetRotation(Mathf.Round(Rotation / 45f) * 45);

            IsRolling = false;
            coroutine = null;

            if (OnRoll != null)
                OnRoll.Invoke();

            if(_accumulatedRoll!=0)
                OnSlopeRoll.Invoke();

            if (!IsDead)
            {
                _astar.AddDynamicBlockTile(gameObject);
            }

            UpdateStandings();

        }

        private IEnumerator RollOnSlope(bool left)
        {
            if (!isActiveAndEnabled)
                yield break;

            if (_astar == null)
                yield break;

            if (IsFalling == true || IsRolling == true)
                yield break;

            if (!IsDead)
            {
                _astar.RemoveDynamicBlockTile(gameObject);
            }

            IsRolling = true;
            IsFalling = false;

            var sign = left ? -1 : 1;

            var bottomId = _astar.GetTileId(transform.position) + new Vector2Int(0, -1);
            var bottomTile = _astar.GetTileData(bottomId);

            var totalRotations = rotations;
            if (!bottomTile.Slope && Rotation % 90 != 0)
                totalRotations += rotations / 2;

            var step = -sign * 90f / rotations;
            var movement = sign / (float) totalRotations;

            var startingPosition = transform.position;
            for (int i = 0; i < totalRotations; i++)
            {
                Rotation += step;
                _rigidbody.MovePosition(transform.position + new Vector3(movement, -Mathf.Abs(movement), 0));
                _rigidbody.SetRotation(Rotation);
                yield return new WaitForSeconds(timeToRoll / (float)rotations);

            }

            SetRotation(Mathf.Round(Rotation / 45f) * 45);
            transform.position = startingPosition + new Vector3(sign, -1, 0);

            IsRolling = false;
            coroutine = null;

            if (OnRoll != null)
                OnRoll.Invoke();
            if (OnSlopeRoll != null)
                OnSlopeRoll.Invoke();

            if (!IsDead)
            {
                _astar.AddDynamicBlockTile(gameObject);
            }

            coroutine = null;
            UpdateStandings();
        }

        public void ResetRotation()
        {
            SetRotation(0);
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
            transform.transform.eulerAngles = new Vector3(0, 0, Rotation);
        }
    }
}
