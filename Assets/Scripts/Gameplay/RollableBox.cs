using System;
using System.Collections;
using UnityEngine;


namespace RuinsRaiders
{

    [DefaultExecutionOrder(100)]
    public class RollableBox : MonoBehaviour
    {
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

        private MovementController _pusher;
        private AStar _astar;
        private Collider2D _collider;

        private float _rotation = 0;
        public bool IsRolling { get; private set; }
        public bool CanRollLeft
        {
            get => !IsFalling && !IsRolling && _astar!=null
                && _astar.GetNode(_astar.GetTileId(transform.position) + new Vector2Int(-1, 0)).Block == false;
        }
        public bool CanRollRight
        {
            get => !IsFalling && !IsRolling && _astar != null
                && _astar.GetNode(_astar.GetTileId(transform.position) + new Vector2Int(1, 0)).Block == false;
        }
        public bool IsFalling { get; private set; }

        private Coroutine coroutine;

        private void Start()
        {
            _astar = FindObjectOfType<AStar>();
            _rotation = transform.rotation.eulerAngles.z;
            _collider = GetComponent<Collider2D>();

            if (_astar == null)
                return;
            _astar.onMapUpdated.AddListener(UpdateStandings);
            _astar.AddDynamicBlockTile(_astar.GetTileId(transform.position));
        }


        private void OnEnable()
        {
            if (_astar == null || _collider == null || _collider.isActiveAndEnabled)
                return;
            _astar.AddDynamicBlockTile(_astar.GetTileId(transform.position));
            UpdateStandings();
        }

        private void OnDisable()
        {
            if (_astar == null)
                return;
            _astar.RemoveDynamicBlockTile(_astar.GetTileId(transform.position));
            UpdateStandings();

        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var pusher = collision.gameObject.GetComponent<MovementController>();
            if (pusher)
            {
                this._pusher = pusher;
                _timeLeftToPush = timeToPush;
            }

        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (_pusher!= null && _pusher.gameObject == collision.gameObject)
            {
                _pusher = null;
                _timeLeftToPush = timeToPush;
            }
        }

        private void FixedUpdate()
        {
            if (coroutine != null)
                return;

            if (_pusher == null)
                return;

            // cannot push when standing on it
            if (_pusher.transform.position.y > transform.position.y - 0.48)
            {
                _timeLeftToPush = timeToPush;
                return;
            }

            // cannot push when not moving
            if (!_pusher.IsMoving)
            {
                _timeLeftToPush = timeToPush;
                return;
            }

            _timeLeftToPush -= Time.fixedDeltaTime;
            if (_timeLeftToPush < 0)
            {
                if (_pusher.faceLeft && !CanRollLeft)
                    return;
                if (!_pusher.faceLeft && !CanRollRight)
                    return;

                coroutine = StartCoroutine(Roll(_pusher.faceLeft));
            }

        }

        public void SetCollision(bool collision)
        {
            if (_collider == null)
                return;

            _collider.enabled = collision;
            if (collision)
            {
                _astar.AddDynamicBlockTile(_astar.GetTileId(transform.position));
            }
            else
            {
                _astar.RemoveDynamicBlockTile(_astar.GetTileId(transform.position));
            }
        }

        public void UpdateStandings()
        {
            if(!isActiveAndEnabled)
                return;

            if (coroutine != null)
                return;

            transform.position = new Vector3(
                (float)Math.Round(transform.position.x * 2, MidpointRounding.AwayFromZero) / 2,
                (float)Math.Round(transform.position.y * 2, MidpointRounding.AwayFromZero) / 2,
                transform.position.z);

            if (_astar == null)
                return;

            var id = _astar.GetTileId(transform.position);
            var bottomTile = _astar.GetNode(id + new Vector2Int(0, -1));
            if (!bottomTile.Block && !bottomTile.Platform)
            {
                coroutine = StartCoroutine(Fall());
                return;
            }

            IsFalling = false;
        }

        private IEnumerator Fall()
        {
            if (IsFalling == true || IsRolling == true)
                yield break;

            IsFalling = true;

            if (isActiveAndEnabled)
                _astar.RemoveDynamicBlockTile(_astar.GetTileId(transform.position));

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

                // stop if reached block or platform
                var id = _astar.GetTileId(transform.position);
                var bottomTile = _astar.GetNode(id + new Vector2Int(0, -1));
                if (bottomTile.Block || bottomTile.Platform)
                {
                    IsFalling = false;
                    break;
                }

                acceleration *= accelearationFactor;
                falledTiles++;
            }


            if (_astar == null)
            {
                coroutine = null;
                yield break;
            }

            if (isActiveAndEnabled)
                _astar.AddDynamicBlockTile(_astar.GetTileId(transform.position));

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

            _astar.RemoveDynamicBlockTile(_astar.GetTileId(transform.position));


            var sign = left ? 1 : -1;
            var step = sign * 90f / (float)rotations;
            var movement = -1 * sign * 1f / (float)rotations;

            for (int i = 0; i < rotations; i++)
            {
                _rotation += step;
                transform.transform.eulerAngles = new Vector3(0, 0, _rotation);
                transform.position += new Vector3(movement, 0, 0);
                yield return new WaitForSeconds(timeToRoll / (float)rotations);
            }

            if (Mathf.Abs(_rotation) > 360)
                _rotation = 0;

            IsRolling = false;

            coroutine = null;

            if (_astar == null)
                yield break;
            _astar.AddDynamicBlockTile(_astar.GetTileId(transform.position));
        }

        public void ResetRotation()
        {
            _rotation = 0;
            transform.transform.eulerAngles = new Vector3(0, 0, _rotation);
        }

    }
}
