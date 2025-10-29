using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Tools.Tilemap
{
    [CreateAssetMenu(fileName = "New Mystery Tile", menuName = "Mystery Tools/Mystery Tile")]
    public class MysteryTile : Tile
    {
        [Header("Properties")]
        [Tooltip("Could be false for something like water.")]
        public bool isWalkable = true;

        [Tooltip("Could be false for decorations like flowers (which the player can walk through).")]
        public bool isSolid = true;

        [Tooltip("NOTE: if this is true, but there is a solid object above it, " +
                 "the player will not be able to jump on top of it. So leaving it as true for walls should be fine.")]
        public bool canJumpOnto = true;

        [Tooltip("Set to true for top slabs or tables.")]
        public bool canCrawlUnder = false;

// #if UNITY_EDITOR
        // TODO: can have it properly visualize stuff that is crawlable maybe? so it is easier to detect if one is unlabeled?
        // needs to have a toggle to turn it off tho...
        //         public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        //         {
        //             base.GetTileData(position, tilemap, ref tileData);

        //             tileData.color = Color.red;
        //         }
// #endif
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MysteryTile))]
    public class MysteryTileEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var spriteProperty = serializedObject.FindProperty("m_Sprite");
            UnityEditor.EditorGUILayout.PropertyField(spriteProperty);
            Sprite sprite = spriteProperty.objectReferenceValue as Sprite;
            Texture2D tex = UnityEditor.AssetPreview.GetAssetPreview(sprite);
            if(sprite != null && tex != null)
            {
                Rect rect = GUILayoutUtility.GetRect(128, 128, GUILayout.ExpandWidth(false));
                UnityEditor.EditorGUI.DrawTextureTransparent(rect, tex);
            }

            DrawPropertiesExcluding(serializedObject,
            "m_Color", "m_Transform", "m_InstancedGameObject",
            "m_Flags", "m_ColliderType", "m_Script", "m_Sprite");

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}