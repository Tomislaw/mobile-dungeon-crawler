using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuinsRaiders
{
    public class Shockwave : MonoBehaviour
    {
        public Projectile shockTile;

        public int shockDamage = 2;

        public int shockWidth;

        public float shockTime;

        public float shockDistance;

        public Vector3 initialOffset;

        private AStar _astar;

        private Character _character;
        private HealthController _healthController;

        void Start()
        {
            _astar = FindAStar();
            _character = GetComponentInChildren<Character>();
            if (_character == null && transform.parent != null)
                _character = transform.parent.GetComponent<Character>();

            _healthController = GetComponentInChildren<HealthController>();
            if (_healthController == null && transform.parent != null)
                _healthController = transform.parent.GetComponent<HealthController>();
        }

        public void StartShockwave()
        {
            StartCoroutine(InitShockwave());
        }

        IEnumerator InitShockwave()
        {
            if (_astar == null)
                yield break;

            var sharedHitTargetsList = new List<HealthController>();
            var startingPosition = initialOffset + transform.position;
            var direction = Mathf.Sign(transform.lossyScale.x);
            startingPosition += new Vector3(direction, 0, 0);

            for (int i = 0; i < shockWidth; i++)
            {
                var id = _astar.GetTileId(startingPosition);

                bool shouldStopShockwave = true;
                for (int y = 1; y > -shockWidth; y--)
                {
                    var nodeBottom = _astar.GetTileData(id + new Vector2Int(0, y - 1));
                    var nodeCenter = _astar.GetTileData(id + new Vector2Int(0, y));

                    var bottomOk = nodeBottom.Block || nodeBottom.Platform;
                    var centerOk = nodeCenter.Destroyable || !nodeCenter.Block;

                    if (bottomOk && centerOk)
                    {
                        startingPosition += new Vector3(0, y, 0);
                        shouldStopShockwave = false;
                        i += Mathf.Abs(y);
                        break;
                    }
                }
                if (shouldStopShockwave)
                    break;

                var prefab = Instantiate(shockTile, startingPosition, Quaternion.identity);
                prefab.launcher = _character;
                prefab.hitTargets = sharedHitTargetsList;
                prefab.group = _healthController.group;
                prefab.damage = shockDamage;

                startingPosition += new Vector3(shockDistance * direction, 0, 0);
                yield return new WaitForSeconds(shockTime);
            }
        }

        private AStar FindAStar()
        {
            var astars = FindObjectsOfType<AStar>().OrderBy(it => Vector2.Distance(it.transform.position, transform.position));
            if (astars.Count() == 0)
                return null;
            return astars.First();
        }
    }
}