using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using _Project.Tools.Tilemap;

namespace _Project.Tools.Tilemap
{
    public class MysteryTilemapHelper
    {
        private List<UnityEngine.Tilemaps.Tilemap> tilemapLayers;

        public MysteryTilemapHelper(List<UnityEngine.Tilemaps.Tilemap> _tilemapLayers)
        {
            tilemapLayers = _tilemapLayers;
        }


        // Helper functions for tile detection
        public bool IsWalkable(Vector3Int _gridPos, int _layer)
        {
            MysteryTile tile = GetTileAt(_gridPos, _layer);
            if (tile == null) return false;

            return tile.isWalkable;
        }

        public bool IsSolid(Vector3Int _gridPos, int _layer)
        {
            MysteryTile tile = GetTileAt(_gridPos, _layer);
            if (tile == null) return false;

            return tile.isSolid;
        }

        public bool CanJumpOnto(Vector3Int _gridPos, int _layer)
        {
            MysteryTile tile = GetTileAt(_gridPos, _layer);
            if (tile == null) return false;

            return tile.canJumpOnto;
        }

        public bool CanCrawlUnder(Vector3Int _gridPos, int _layer)
        {
            MysteryTile tile = GetTileAt(_gridPos, _layer);
            // Having this default to true, if they are crawling they should be able to move through empty space
            // That being said, it should be allowed by an IsSolid() check as well
            if (tile == null) return true; 

            return tile.canCrawlUnder;
        }
        
        public MysteryTile GetTileAt(Vector3Int _gridPos, int _layer)
        {
            if (_layer < 0 || _layer >= tilemapLayers.Count) return null;

            MysteryTile tile = tilemapLayers[_layer].GetTile<MysteryTile>(_gridPos);
            return tile;
        }
    }
}
