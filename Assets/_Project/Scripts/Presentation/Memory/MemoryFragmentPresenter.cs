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
            Debug.Log("MemoryFragmentPresenter: Retrieving IMemoryNpcMoveService from ServiceLocater...");
            var npcService = ServiceLocater.GetService<IMemoryNpcMoveService>();
            var dialogueService = ServiceLocater.GetService<IMemoryDialogueService>();

            var context = new MemoryActionContext(dialogueService, npcService);
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
}