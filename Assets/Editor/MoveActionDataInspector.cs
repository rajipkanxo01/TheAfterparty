using _Project.Scripts.Data.Memory.Actions;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(MoveActionBaseData))]
    public class MoveActionDataInspector : UnityEditor.Editor
    {
        private Transform _tempPoint;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MoveActionBaseData data = (MoveActionBaseData)target;

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Add Path Point (Drag a Transform)", EditorStyles.boldLabel);

            _tempPoint = (Transform)EditorGUILayout.ObjectField("Transform:", _tempPoint, typeof(Transform), true);

            if (_tempPoint != null)
            {
                // Add its position to the Vector3 array
                Undo.RecordObject(data, "Add Path Point");

                ArrayUtility.Add(ref data.paths, _tempPoint.position);

                // Clear reference so user can add next one
                _tempPoint = null;

                EditorUtility.SetDirty(data);
            }
        }
    }
}