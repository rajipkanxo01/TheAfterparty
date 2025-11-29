using System.IO;
using _Project.Scripts.Data.Memory.Actions;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor
{
    public class MemoryFragmentToolWindow : EditorWindow
    {
        private FragmentData selectedFragment;
        private bool showReal = true;
        private Vector2 scrollPos;
        private ReorderableList reorderableList;

        [MenuItem("Tools/Memory Fragment Tool")]
        public static void OpenWindow()
        {
            GetWindow<MemoryFragmentToolWindow>("Memory Fragment Tool");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Memory Fragment Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            selectedFragment =
                (FragmentData)EditorGUILayout.ObjectField("Fragment", selectedFragment, typeof(FragmentData), false);

            if (selectedFragment == null)
            {
                EditorGUILayout.HelpBox("Select a FragmentData asset to begin.", MessageType.Info);
                return;
            }

            showReal = GUILayout.Toggle(showReal,
                showReal ? "Showing Real Memory Actions" : "Showing Corrupted Memory Actions", "Button");

            var list = showReal ? selectedFragment.realMemoryActions : selectedFragment.corruptedMemoryActions;

            EnsureReorderableList(list);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            reorderableList.DoLayoutList();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            DrawAddButtons(list);

            if (GUI.changed)
                EditorUtility.SetDirty(selectedFragment);
        }

        private void EnsureReorderableList(System.Collections.Generic.List<ActionBaseData> actions)
        {
            if (reorderableList == null || reorderableList.list != actions)
            {
                reorderableList = new ReorderableList(actions, typeof(ActionBaseData), true, true, true, true);

                reorderableList.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, showReal ? "Real Memory Actions" : "Corrupted Memory Actions");
                };

                reorderableList.drawElementCallback = (rect, index, active, focused) =>
                {
                    if (index >= actions.Count) return;
                    var action = actions[index];
                    if (action == null)
                    {
                        EditorGUI.LabelField(rect, "⚠ Null Action (remove it)");
                        return;
                    }

                    float y = rect.y + 4f; // top padding
                    float line = EditorGUIUtility.singleLineHeight + 3f; // spacing between lines
                    float labelWidth = 80f;
                    float fullWidth = rect.width;
                    float halfWidth = fullWidth * 0.48f;

                    // --- Title / Header ---
                    GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
                    headerStyle.normal.textColor = new Color(0.8f, 0.9f, 1f);

                    float labelWidthRename = 25f;
                    float nameFieldWidth = fullWidth - labelWidthRename - 10f;

                    EditorGUI.LabelField(new Rect(rect.x, y, labelWidthRename, line), $"{index + 1}.", headerStyle);

                    string newName = EditorGUI.DelayedTextField(
                        new Rect(rect.x + labelWidthRename + 5f, y, nameFieldWidth, line),
                        action.name
                    );

                    if (newName != action.name && !string.IsNullOrEmpty(newName))
                    {
                        string path = AssetDatabase.GetAssetPath(action);
                        if (!string.IsNullOrEmpty(path))
                        {
                            AssetDatabase.RenameAsset(path, newName);
                            action.name = newName;
                            EditorUtility.SetDirty(action);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                    }
                    y += line;



                    // --- Reference + Play Mode ---
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.ObjectField(new Rect(rect.x, y, halfWidth, line), "Asset", action, typeof(ActionBaseData),
                        false);
                    action.playMode = (ActionPlayMode)EditorGUI.EnumPopup(
                        new Rect(rect.x + halfWidth + 10f, y, halfWidth - 10f, line),
                        "Play Mode", action.playMode);
                    y += line;

                    // --- Actor / Yarn Info ---
                    var move = action as BezierMoveActionBaseData;
                    var dialogue = action as DialogueActionData;

                    if (move != null)
                    {
                        move.npcId = EditorGUI.TextField(new Rect(rect.x, y, halfWidth, line), "Actor ID",
                            move.npcId);
                        y += line;
                    }
                    else if (dialogue != null)
                    {
                        dialogue.npcId = EditorGUI.TextField(new Rect(rect.x, y, halfWidth, line), "Actor ID",
                            dialogue.npcId);
                        dialogue.yarnNodeName =
                            EditorGUI.TextField(new Rect(rect.x + halfWidth + 10f, y, halfWidth - 10f, line),
                                "Yarn Node", dialogue.yarnNodeName);
                        y += line;
                    }

                    // --- Timing ---
                    action.startDelaySeconds = EditorGUI.FloatField(new Rect(rect.x, y, halfWidth, line), "Delay (s)",
                        action.startDelaySeconds);
                    action.order = index;
                    y += line;

                    // --- Bottom margin ---
                    rect.height = y - rect.y + 6f;

                    if (EditorGUI.EndChangeCheck())
                        EditorUtility.SetDirty(action);
                };

                reorderableList.elementHeightCallback = (index) =>
                {
                    var action = (ActionBaseData)reorderableList.list[index];
                    float lines = 3; // base lines (header + asset/playmode + delay)
                    if (action is BezierMoveActionBaseData) lines += 1;
                    if (action is DialogueActionData) lines += 1;
                    return lines * (EditorGUIUtility.singleLineHeight + 3f) + 8f;
                };


                reorderableList.onAddCallback = l =>
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add Move Action"), false,
                        () => CreateAndAddAction<BezierMoveActionBaseData>(actions, "MoveAction_"));
                    menu.AddItem(new GUIContent("Add Dialogue Action"), false,
                        () => CreateAndAddAction<DialogueActionData>(actions, "DialogueAction_"));
                    menu.ShowAsContext();
                };

                reorderableList.onRemoveCallback = l =>
                {
                    if (l.index >= 0 && l.index < actions.Count)
                    {
                        if (EditorUtility.DisplayDialog("Remove Action", $"Remove {actions[l.index].name}?", "Yes",
                                "Cancel"))
                        {
                            actions.RemoveAt(l.index);
                        }
                    }
                };

                reorderableList.onReorderCallback = l =>
                {
                    for (int i = 0; i < actions.Count; i++)
                        if (actions[i] != null)
                            actions[i].order = i;

                    EditorUtility.SetDirty(selectedFragment);
                };
            }
        }

        private void DrawAddButtons(System.Collections.Generic.List<ActionBaseData> list)
        {
            EditorGUILayout.LabelField("Add New Action", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Move Action"))
            {
                CreateAndAddAction<BezierMoveActionBaseData>(list, "MoveAction_");
            }

            if (GUILayout.Button("Add Dialogue Action"))
            {
                CreateAndAddAction<DialogueActionData>(list, "DialogueAction_");
            }

            EditorGUILayout.EndHorizontal();
        }

        private void CreateAndAddAction<T>(System.Collections.Generic.List<ActionBaseData> list, string prefix)
            where T : ActionBaseData
        {
            string fragmentPath = AssetDatabase.GetAssetPath(selectedFragment);
            string fragmentDir = Path.GetDirectoryName(fragmentPath);
            string assetName = prefix + list.Count + ".asset";

            T newAction = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(newAction, Path.Combine(fragmentDir, assetName));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            list.Add(newAction);
            EditorUtility.SetDirty(selectedFragment);
            Selection.activeObject = newAction;
        }
    }
}