using _Project.Scripts.Data.Memory.Actions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BezierMoveActionBaseData))]
    public class BezierMoveActionDataInspector : UnityEditor.Editor
    {
        private Transform _tempPoint;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BezierMoveActionBaseData data = (BezierMoveActionBaseData)target;

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Add Path Point [and its children as control points] (Drag a Transform)", EditorStyles.boldLabel);

            _tempPoint = (Transform)EditorGUILayout.ObjectField("Transform:", _tempPoint, typeof(Transform), true);

            if (_tempPoint != null)
            {
                // Add its position to the Vector3 array
                Undo.RecordObject(data, "Add Bezier Path Point");

                BezierPath bezierPath = new BezierPath();
                foreach (Transform controlPoint in _tempPoint)
                {
                    bezierPath.controlPoints.Add(controlPoint.position);
                }
                bezierPath.endPoint = _tempPoint.position;

                ArrayUtility.Add(ref data.paths, bezierPath);

                // Clear reference so user can add next one
                _tempPoint = null;

                EditorUtility.SetDirty(data);
            }
        }
    }
}