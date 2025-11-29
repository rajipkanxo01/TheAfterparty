using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using _Project.Scripts.Data.Memory.Actions;

namespace _Project.Tools.Animation
{
#if UNITY_EDITOR

    public class MysteryMovementGenerator : EditorWindow
    {
        private Transform fragmentParent = null;
        private string npcID = "";
        private string outputFolder = "Assets/_Project/ScriptableObjects/Memory";

        [MenuItem("Mystery Tools/Mystery Movement Generator")]
        public static void ShowWindow()
        {
            GetWindow<MysteryMovementGenerator>("Mystery Movement Generator");
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Please drag a FRAGMENT_NAME from the scene's \"NPCs/Paths/NPC_NAME/FRAGMENT_NAME\" into the field below." +
                                    "This will create MoveActions for each of it's children.", MessageType.Info);
            fragmentParent = (Transform)EditorGUILayout.ObjectField("Fragment", fragmentParent, typeof(Transform), true);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Please enter the NPCs id here, so far we have: detective, girl, mascot, mouse.", MessageType.Info);
            npcID = EditorGUILayout.TextField("Npc id", npcID);

            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets/_Project/ScriptableObjects/Memory", "");
                outputFolder = "Assets" + path.Substring(Application.dataPath.Length);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUI.enabled = fragmentParent != null;
            if (GUILayout.Button("Generate Move Actions", GUILayout.Height(40)))
            {
                GenerateMoveActions();
            }
            GUI.enabled = true;
        }

        private void GenerateMoveActions()
        {
            int i = 0;
            foreach (Transform path in fragmentParent)
            {
                BezierMoveActionBaseData moveAction = CreateInstance<BezierMoveActionBaseData>();
                moveAction.npcId= npcID;
                BezierPath bezierPath = new BezierPath();
                bezierPath.endPoint = path.position;
                foreach(Transform target in path)
                {
                    bezierPath.controlPoints.Add(target.position);
                }
                moveAction.path = bezierPath;

                string assetName = "";
                if (i == 0) assetName = "START-" + npcID + "_" + fragmentParent.name + ".asset";
                else assetName = npcID + "-" + i.ToString() + "_" + path.name + "_" + fragmentParent.name + ".asset";
                
                string assetPath = Path.Combine(outputFolder, assetName);
                AssetDatabase.CreateAsset(moveAction, assetPath);
                ++i;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Close();
            EditorUtility.DisplayDialog("Generated move actions!", "Successfully created move actions for " + npcID + " - " + fragmentParent.name, "OK");
        }
    }

#endif
}
