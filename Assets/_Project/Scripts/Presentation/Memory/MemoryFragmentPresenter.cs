using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory;
using _Project.Scripts.Application.Memory.Services;
using _Project.Scripts.Data.Memory;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Presentation.Memory
{
    public class MemoryFragmentPresenter : MonoBehaviour
    {
        [SerializeField] private string memoryId;

        private MemoryFragmentExecutor _executor;
        private MemoryDatabase _memoryDatabase;
        private List<FragmentData> _fragmentsToPlay;
        private string _currentFragmentId;
        private int _currentIndex = 0;
        
        public string CurrentFragmentId => _currentFragmentId;
        private bool IsPlaying { get; set; }

        private void Awake()
        {
        }

        private void Start()
        {
            _memoryDatabase = ServiceLocater.GetService<MemoryDatabase>();

            if (_memoryDatabase is null)
            {
                Debug.LogError("MemoryFragmentPresenter: MemoryDatabase not found!");
                return;
            }
            
            var npcService = ServiceLocater.GetService<IMemoryNpcMoveService>();
            var dialogueService = ServiceLocater.GetService<IMemoryDialogueService>();

            var context = new MemoryActionContext(dialogueService, npcService);
            _executor = new MemoryFragmentExecutor(context);

            var memoryData = _memoryDatabase.GetById(memoryId);
            if (memoryData == null)
            {
                Debug.LogError($"MemoryFragmentPresenter: Memory with ID '{memoryId}' not found!");
                return;
            }
            
            _fragmentsToPlay = memoryData.fragments;
        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsPlaying)
                {
                    Debug.Log("MemoryFragmentPresenter: Already playing a fragment.");
                    return;
                }
                
                if (_fragmentsToPlay == null || _fragmentsToPlay.Count == 0)
                {
                    Debug.LogWarning("MemoryFragmentPresenter: No fragments to play.");
                    return;
                }
                
                if (_currentIndex >= _fragmentsToPlay.Count)
                {
                    Debug.Log("MemoryFragmentPresenter: All fragments have been played.");
                    return;
                }

                var fragment = _fragmentsToPlay[_currentIndex];
                await PlayFragmentAsync(fragment);
                _currentIndex++;
            }
        }
        
        private async Task PlayFragmentAsync(FragmentData fragment)
        {
            IsPlaying = true;
            _currentFragmentId = fragment.fragmentId;

            Debug.Log($"[MemoryFragmentPresenter] ▶ Playing fragment: {_currentFragmentId}");
            await _executor.PlayFragmentAsync(fragment);
            Debug.Log($"[MemoryFragmentPresenter] ✅ Finished fragment: {_currentFragmentId}");

            _currentFragmentId = null;
            IsPlaying = false;

            if (_currentIndex + 1 < _fragmentsToPlay.Count)
            {
                Debug.Log($"MemoryFragmentPresenter: Press SPACE to play next fragment ({_currentIndex + 1}/{_fragmentsToPlay.Count}).");
            }
        }
    }
}