using UnityEngine;

namespace _Project.Scripts.Presentation.MemoryPointer
{
    public class OffscreenPointer : MonoBehaviour
    {
        public float edgeBuffer = 40f;
        public float fadeSpeed = 10f;

        [HideInInspector] public Transform target;

        private Camera _cam;
        private RectTransform _rect;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _cam = Camera.main;
            _rect = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
                return;
            }

            Vector3 screenPos = _cam.WorldToScreenPoint(target.position);
            bool behindCamera = screenPos.z < 0;

            bool offScreen = screenPos.x < 0 || screenPos.x > Screen.width ||
                             screenPos.y < 0 || screenPos.y > Screen.height ||
                             behindCamera;

            if (offScreen)
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, 1f, Time.deltaTime * fadeSpeed);

                if (behindCamera)
                {
                    screenPos *= -1f;
                }

                screenPos.x = Mathf.Clamp(screenPos.x, edgeBuffer, Screen.width - edgeBuffer);
                screenPos.y = Mathf.Clamp(screenPos.y, edgeBuffer, Screen.height - edgeBuffer);

                _rect.position = screenPos;

                var dir = (Vector2)(screenPos - new Vector3(Screen.width/2f, Screen.height/2f));
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; ;
                _rect.rotation = Quaternion.Euler(0,0,angle);
            }
            else
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
            }
        }

        public void SetTarget(Transform t)
        {
            target = t;
        }
    }
}