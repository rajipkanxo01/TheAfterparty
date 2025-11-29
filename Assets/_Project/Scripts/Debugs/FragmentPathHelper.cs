using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class FragmentPathHelper : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("NOTE: the first child will be assumed\nto be a start position with no path")]
    [Space()]
    [SerializeField] private bool _showPaths;
#endif

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (_showPaths)
        {
            Transform prev = null;
            float hue = 0.98f;
            foreach (Transform pathParent in transform)
            {
                Gizmos.color = Color.HSVToRGB(hue, 0.9f, 0.8f);
                Gizmos.DrawSphere(pathParent.position, 0.2f);
                if (prev != null)
                {
                    foreach(Transform pathPoint in pathParent)
                    {
                        Gizmos.DrawLine(prev.position, pathPoint.position);
                        Gizmos.DrawSphere(pathPoint.position, 0.1f);
                        prev = pathPoint;
                    }
                    Gizmos.DrawLine(prev.position, pathParent.position);
                }
                hue = (hue + 0.12f) % 1;
                prev = pathParent;
            }
        }
#endif
    }
}
