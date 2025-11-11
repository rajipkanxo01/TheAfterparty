using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Presentation.Npc
{
    public class NpcController : MonoBehaviour
    {
        [SerializeField] private string npcId;
        [SerializeField] private float defaultSpeed = 2f;
        
        public string NpcId => npcId;
        
        public Task MoveAlongPathAsync(IReadOnlyList<Vector3> positions, float speed)
        {
            if (positions == null || positions.Count == 0)
            {
                return Task.CompletedTask;
            }

            if (speed <= 0f)
            {
                speed = defaultSpeed;
            }

            var tcs = new TaskCompletionSource<bool>();
            StartCoroutine(MovePathCoroutine(positions, speed, tcs));
            return tcs.Task;
        }

        private IEnumerator MovePathCoroutine(IReadOnlyList<Vector3> positions, float speed, TaskCompletionSource<bool> tcs)
        {
            foreach (var target in positions)
            {
                yield return MoveTo(target, speed);
            }

            tcs.TrySetResult(true);
        }

        private IEnumerator MoveTo(Vector3 target, float speed)
        {
            Vector3 start = transform.position;
            float distance = Vector3.Distance(start, target);

            if (distance < 0.001f)
            {
                yield break;
            }

            float duration = distance / speed;
            float elapsed = 0f;

            // todo: add walk animation trigger here

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transform.position = Vector3.Lerp(start, target, t);
                yield return null;
            }

            transform.position = target;

            // todo: reset to idle animation here
        }
    }
}