using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{

    [DefaultExecutionOrder(100)]
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
        private HealthController _healthController;

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
            return _astar.GetNode(id + new Vector2Int(sign, 0)).Block == false
                && !IsSlope(id + new Vector2Int(sign, 0));
        }


        public bool IsFalling { get; private set; }

        private Coroutine coroutine;


        private void Start()
        {
            _astar = FindObjectOfType<AStar>();
            Rotation = transform.rotation.eulerAngles.z;
            _collider = GetComponent<Collider2D>();
            _healthController = GetComponent<HealthController>();

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

                coroutine = StartCoroutine(Roll(pusher.faceLeft));
            }

        }

        public void UpdateStandings()
        {
            if(!isActiveAndEnabled)
                return;

            if (coroutine != null)
                return;

            if (IsDead && _collider != null && _collider.enabled == true) { 
                _collider.enabled = false;
                ResetRotation();
                _astar.RemoveDynamicBlockTile(gameObject);
            }

            SnapToGrid();

            if (_astar == null)
                return;

            var id = _astar.GetTileId(transform.position) + new Vector2Int(0, -1);
            var bottomNode = _astar.GetNode(id);
            var isBottomSlope = IsSlope(id);
            if (isBottomSlope && !IsDead)
            {
                if(CanRoll(true, id))
                    coroutine = StartCoroutine(RollOnSlope(true));
                else if (CanRoll(false, id))
                    coroutine = StartCoroutine(RollOnSlope(false));
            } else if (!bottomNode.Block && !bottomNode.Platform)
            {
                coroutine = StartCoroutine(Fall());
            }

        }

        public void SnapToGrid()
        {
            transform.position = new Vector3(
                (float)Math.Round(transform.position.x * 2, MidpointRounding.AwayFromZero) / 2,
                (float)Math.Round(transform.position.y * 2, MidpointRounding.AwayFromZero) / 2,
                transform.position.z);
        }

        private IEnumerator Fall()
        {
            if (IsFalling == true || IsRolling == true)
                yield break;

            IsFalling = true;

            if (!IsDead)
            {
                _astar.RemoveDynamicBlockTile(gameObject);
            }

            int falledTiles = 0;
            var movement = 1f / fallPoints;

            var acceleration = 1f;
            var accelearationFactor = 1f - accelerationPercentage;

            while (IsFalling)
            {

                for (int i = 0; i < fallPoints; i++)
                {
                    transform.position -= new Vector3(0, movement, 0);
                    yield return new WaitForSeconds((timeToFall / (float)fallPoints) * acceleration);
                }

                if(OnFall != null)
                    OnFall.Invoke();

                    // stop if reached block or platform
                var bottomId = _astar.GetTileId(transform.position) + new Vector2Int(0, -1);
                var bottomTile = _astar.GetNode(bottomId);

                if (bottomTile.Block || bottomTile.Platform || IsSlope(bottomId))
                {
                    IsFalling = false;
                    break;
                }

                acceleration *= accelearationFactor;
                falledTiles++;
            }

            yield return new WaitForSeconds((timeToFall / (float)fallPoints));


            if (!IsDead)
            {
                _astar.AddDynamicBlockTile(gameObject);
            }

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

            for (int i = 0; i < rotations; i++)
            {
                yield return new WaitForSeconds(timeToRoll / (float)rotations);

                Rotation += step;
                transform.transform.eulerAngles = new Vector3(0, 0, Rotation);
                transform.position += new Vector3(movement, 0, 0);

            }

            if (Mathf.Abs(Rotation) > 360)
                Rotation = 0;

            IsRolling = false;
            coroutine = null;

            if (OnRoll != null)
                OnRoll.Invoke();

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

            var id = _astar.GetTileId(transform.position);
            var isBottomSlope = IsSlope(id + new Vector2Int(0, -1));

            var totalRotations = rotations;
            if (!isBottomSlope)
                totalRotations += rotations / 2;

            var step = -sign * 90f / rotations;
            var movement = sign / (float) totalRotations;

            for (int i = 0; i < totalRotations; i++)
            {
                yield return new WaitForSeconds(timeToRoll / (float)rotations);

                Rotation += step;
                transform.transform.eulerAngles = new Vector3(0, 0, Rotation);
                transform.position += new Vector3(movement, -Mathf.Abs(movement), 0);

            }

            Rotation = Mathf.Round(Rotation / 45f) * 45;


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
            ResetRotation(0);
        }

        public void ResetRotation(float rotation)
        {
            Rotation = rotation;
            transform.transform.eulerAngles = new Vector3(0, 0, Rotation);
        }

        private bool IsSlope(Vector2Int Id)
        {
            var tile = _astar.GetTile(Id);

            if(tile==null)
                return false;
            else 
                return tile.name.Contains("Slope");
        }
    }
}
