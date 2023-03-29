using System.Collections;
using System.Linq;
using UnityEngine;

namespace RuinsRaiders
{
    public class Shockwave : MonoBehaviour
    {
        [SerializeField]
        private GameObject shockTile;

        [SerializeField]
        private int shockWidth;
        [SerializeField]
        private float shockTime;


        [SerializeField]
        private float shockDistance;
        [SerializeField]
        private Vector3 initialOffset;

        private AStar _astar;

        void Start()
        {
            _astar = FindAStar();
        }

        public void StartShockwave()
        {
            StartCoroutine(InitShockwave());
        }

        IEnumerator InitShockwave()
        {
            if (_astar == null)
                yield break;

            var startingPosition = initialOffset + transform.position;
            var rotation = Mathf.Sign(transform.localScale.x);
            startingPosition += new Vector3(rotation, 0, 0);
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
                var projectile = prefab.GetComponent<Projectile>();
                if (projectile != null)
                    projectile.launcher = GetComponent<Character>();

                startingPosition += new Vector3(shockDistance * rotation, 0, 0);
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