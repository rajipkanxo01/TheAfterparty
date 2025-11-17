using _Project.Scripts.Application.Badge;
using UnityEngine;

namespace _Project.Scripts.Presentation.Badge
{
    public class AnimatedBadgePickup : MonoBehaviour
    {
        [SerializeField] private float riseHeight = 1.5f;
        [SerializeField] private float riseDuration = 1f;
        [SerializeField] private float autoCollectDelay = 0.3f;

        private Vector3 _startPos;
        private Vector3 _endPos;
        private float _timer;
        private bool _isRising = true;

        private void Start()
        {
            _startPos = transform.position;
            _endPos = _startPos + Vector3.up * riseHeight;
        }

        private void Update()
        {
            if (_isRising)
            {
                _timer += Time.deltaTime;
                float t = _timer / riseDuration;

                transform.position = Vector3.Lerp(_startPos, _endPos, t);

                if (t >= 1f)
                {
                    _isRising = false;
                    Invoke(nameof(AutoCollect), autoCollectDelay);
                }
            }
        }

        private void AutoCollect()
        {
            BadgeEvents.RaiseBadgePicked();
            Destroy(gameObject);
        }
    }
}