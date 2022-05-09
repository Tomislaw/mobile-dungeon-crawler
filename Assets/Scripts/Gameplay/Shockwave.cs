using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Shockwave : MonoBehaviour
{
    public GameObject shockTile;
    public int shockWidth;
    public float shockTime;
    private AStar astar;
    public float shockDistance;
    public Vector3 InitialOffset;

    void Start()
    {
        astar = FindAStar();
    }

    public void StartShockwave()
    {
        StartCoroutine(InitShockwave());
    }

    IEnumerator InitShockwave()
    {
        if (astar == null)
            yield break;

        var startingPosition = InitialOffset + transform.position;
        var rotation = Mathf.Sign(transform.localScale.x);
        startingPosition += new Vector3(rotation, 0, 0);
        for (int i = 0; i < shockWidth; i++)
        {
            var id = astar.GetTileId(startingPosition);

            bool shouldStopShockwave = true;
            for (int y = 1; y > -shockWidth; y--)
            {
                var nodeBottom = astar.GetNode(id + new Vector2Int(0, y - 1));
                var nodeCenter = astar.GetNode(id + new Vector2Int(0, y));

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
                projectile.Launcher = GetComponent<Character>();

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
