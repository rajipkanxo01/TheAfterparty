using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Data.Memory.Actions
{
    [System.Serializable]
    public class BezierPath
    {
        public List<Vector3> controlPoints = new List<Vector3>();
        public Vector3 endPoint;
    }
}
