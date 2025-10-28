using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Scripts.Application.Player
{
    public class TileOpacityManager
    {
        // Tilemap storage for collisions
        // private List<Material> tilemapMaterials = new List<Material>();
        // private int currentLayer;


        // public TileOpacityManager(List<Tilemap> _tilemapLayers, int _currentLayer, Material tilemapMat)
        // {
        //     foreach (Tilemap tilemap in _tilemapLayers)
        //     {
        //         // Need to make a new material for each layer so they can be modified seperately
        //         TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        //         Material mat = new Material(tilemapMat);
        //         tilemapMaterials.Add(mat);
        //         renderer.material = mat;
        //     }

        //     currentLayer = _currentLayer;
        // }

        // public void UpdatePosition(Vector2 position)
        // {
        //     foreach (Material mat in tilemapMaterials)
        //     {
        //         mat.SetVector("_playerPosition", position);
        //     }
        // }
        
        // void OnDestroy()
        // {
        //     foreach(Material mat in tilemapMaterials)
        //     {
        //         if(mat != null) 
        //     }
        // }
    }
}
