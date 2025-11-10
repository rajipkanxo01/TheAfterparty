using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory;
using _Project.Scripts.Application.Memory.Services;
using _Project.Scripts.Data.Memory.Fragments;
using _Project.Scripts.Presentation.Memory.Services;
using UnityEngine;

namespace _Project.Scripts.Presentation.Memory
{
    public class MemoryFragmentPresenter : MonoBehaviour
    {
        [Header("Fragment Reference")] [SerializeField]
        private FragmentData fragmentToPlay;

        private MemoryFragmentExecutor _executor;

        private void Awake()
        {
        }

        private void Start()
        {
            var dialogueService = gameObject.AddComponent<MemoryDialogueService>();
            ServiceLocater.RegisterService<IMemoryDialogueService>(dialogueService);
            
            IMemoryActorService actorService = new DebugActorService();
            ServiceLocater.RegisterService<IMemoryActorService>(actorService);

            var context = new MemoryActionContext(dialogueService, actorService);
            _executor = new MemoryFragmentExecutor(context);

            if (fragmentToPlay != null)
            {
                Debug.Log($"[MemoryFragmentPresenter] Ready to play fragment '{fragmentToPlay.fragmentId}'. Press SPACE to start.");
            }
            else
            {
                Debug.LogWarning("[MemoryFragmentPresenter] No fragment assigned in the Inspector!");
            }
        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (fragmentToPlay == null)
                {
                    Debug.LogWarning("[MemoryFragmentPresenter] No fragment assigned!");
                    return;
                }

                Debug.Log($"[MemoryFragmentPresenter] ▶ Starting fragment: {fragmentToPlay.fragmentId}");

                await _executor.PlayFragmentAsync(fragmentToPlay);

                Debug.Log($"[MemoryFragmentPresenter] ✅ Finished fragment: {fragmentToPlay.fragmentId}");
            }
        }
    }

    #region Mock Debug Services

    /// <summary>
    /// Simple console-only movement handler for testing.
    /// </summary>
    public class DebugActorService : IMemoryActorService
    {
        public async Task MoveAlongPathAsync(string actorId, IReadOnlyList<Vector3> positions, float speed)
        {
            Debug.Log($"[Move] Actor '{actorId}' moving through {positions.Count} points at speed {speed}");

            foreach (var pos in positions)
            {
                Debug.Log($"[Move] → Moving {actorId} to {pos}");
                await Task.Delay(500);
            }

            Debug.Log($"[Move] '{actorId}' finished path");
        }
    }

    #endregion
}