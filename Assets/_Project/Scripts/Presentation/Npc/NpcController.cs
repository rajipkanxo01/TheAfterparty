using _Project.Scripts.Data.Memory.Actions;
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
        
        // NOTE: currently needs to be serialized as NPC sprite is not on this game object
        // and I'm not sure FindInChildren will return one spritecomponent forever
        [SerializeField] private Animator animator;

        public string NpcId => npcId;
        
        public Task MoveAlongPathAsync(BezierPath path, float speed)
        {
            if (path == null)
            {
                return Task.CompletedTask;
            }

            if (speed <= 0f)
            {
                speed = defaultSpeed;
            }

            var tcs = new TaskCompletionSource<bool>();
            StartCoroutine(MovePathCoroutine(path, speed, tcs));
            return tcs.Task;
        }

        private IEnumerator MovePathCoroutine(BezierPath path, float speed, TaskCompletionSource<bool> tcs)
        {
            foreach (var target in path.controlPoints)
            {
                yield return MoveTo(target, speed);
            }
            yield return MoveTo(path.endPoint, speed);

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

            if (animator)
            {
                animator.SetFloat("moveX", target.x - start.x);
                animator.SetFloat("moveY", target.y - start.y);
            }

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