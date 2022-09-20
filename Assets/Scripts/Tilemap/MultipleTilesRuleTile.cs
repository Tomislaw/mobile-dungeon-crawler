using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RuinsRaiders
{
    [CreateAssetMenu(fileName = "MultipleTilesRuleTile", menuName = "RuinsRaiders/MultipleTilesRuleTile", order = 1)]
    public class MultipleTilesRuleTile : RuleTile<MultipleTilesRuleTile.Neighbor>
    {
        public List<string> tiles = new();

        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            public const int AnyFromList = 3;
            public const int AnyNotFromList = 4;
        }

        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            switch (neighbor)
            {
                case Neighbor.AnyFromList: return tile != null && tiles.Exists(it => it == tile.name);
                case Neighbor.AnyNotFromList:
                    if (tile == null)
                        return !tiles.Exists(it => it == "null");
                    else
                        return !tiles.Exists(it => it == tile.name);
            }
            return base.RuleMatch(neighbor, tile);
        }
    }
}