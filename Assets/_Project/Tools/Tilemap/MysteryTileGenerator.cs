using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using System.IO;

namespace _Project.Tools.Tilemap
{
    public class MysteryTileGenerator : EditorWindow
    {
        private Texture2D spritesheet;
        private string outputFolder = "Assets/_Project/Art";

        [MenuItem("Mystery Tools/Mystery Tile Generator")]
        public static void ShowWindow()
        {
            GetWindow<MysteryTileGenerator>("Mystery Tile Generator");
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Make sure the spritesheet is sliced into multiple sprites first.", MessageType.Info);
            spritesheet = (Texture2D)EditorGUILayout.ObjectField("Spritesheet", spritesheet, typeof(Texture2D), false);

            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets/_Project/Art", "");
                outputFolder = "Assets" + path.Substring(Application.dataPath.Length);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUI.enabled = spritesheet != null;
            if (GUILayout.Button("Generate Tiles", GUILayout.Height(40)))
            {
                GenerateTiles();
            }
            GUI.enabled = true;
        }

        private void GenerateTiles()
        {
            string spritesheetPath = AssetDatabase.GetAssetPath(spritesheet);
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(spritesheetPath);

            List<Sprite> sprites = new List<Sprite>();
            foreach (Object obj in objs)
            {
                if (obj is Sprite) sprites.Add(obj as Sprite);
            }

            if (sprites.Count == 0)
            {
                Debug.LogError("No sprites found... uh oh");
                return;
            }

            foreach (Sprite sprite in sprites)
            {
                MysteryTile tile = CreateInstance<MysteryTile>();
                tile.sprite = sprite;

                string tilePath = Path.Combine(outputFolder, sprite.name + ".asset");
                AssetDatabase.CreateAsset(tile, tilePath);

                // Set icon of tile
                
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
