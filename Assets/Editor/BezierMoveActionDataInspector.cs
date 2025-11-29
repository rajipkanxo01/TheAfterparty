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

            //EditorGUILayout.Space(10);

            GUIStyle textStyle = new GUIStyle(EditorStyles.label);
            textStyle.wordWrap = true;
            textStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("Add a move path (will move through targets in order, then reach end point). The GameObject structure should be:\n\n" +
                                        "END_POINT\n" +
                                        "-> TARGET_1\n" +
                                        "-> TARGET_2\n" +
                                        "-> ...\n\n" +
                                        "Please drag 'PATH_NAME' into the field below to add all paths.",
                                        textStyle);

            _tempPoint = (Transform)EditorGUILayout.ObjectField("Transform:", _tempPoint, typeof(Transform), true);

            if (_tempPoint != null)
            {
                // Add its position to the Vector3 array
                Undo.RecordObject(data, "Add Bezier Path Points");

                BezierPath bezierPath = new BezierPath();
                foreach (Transform controlPoint in _tempPoint)
                {
                    bezierPath.controlPoints.Add(controlPoint.position);
                }
                bezierPath.endPoint = _tempPoint.position;

                data.path = bezierPath;

                // Clear reference so user can add next one
                _tempPoint = null;

                EditorUtility.SetDirty(data);
            }
        }
    }
}