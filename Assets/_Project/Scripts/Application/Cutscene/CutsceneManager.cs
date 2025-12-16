using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Cutscene.Events;
using _Project.Scripts.Application.Memory;
using _Project.Scripts.Application.Memory.Services;
using _Project.Scripts.Data.Cutscene;
using _Project.Scripts.Presentation.Memory.Services;
using UnityEngine;
using UnityEngine;

namespace _Project.Scripts.Application.Cutscene
{
    public class CutsceneManager : MonoBehaviour
    {
        [Header("Cutscene Database")]
        [SerializeField] private List<CutsceneData> availableCutscenes = new();
        
        private CutsceneExecutor _executor;
        private Dictionary<string, CutsceneData> _cutsceneLookup;

        private void Awake()
        {
            InitializeCutsceneLookup();
        }

        private void Start()
        {
            InitializeExecutor();
            
            // Subscribe to cutscene play requests
            CutsceneEvents.OnPlayCutsceneRequested += HandlePlayCutsceneRequest;
        }

        private void OnDestroy()
        {
            CutsceneEvents.OnPlayCutsceneRequested -= HandlePlayCutsceneRequest;
            _executor?.Dispose();
        }

        private void InitializeCutsceneLookup()
        {
            _cutsceneLookup = new Dictionary<string, CutsceneData>();
            
            foreach (var cutscene in availableCutscenes)
            {
                if (cutscene == null)
                {
                    Debug.LogWarning("CutsceneManager: Null cutscene in available cutscenes list");
                    continue;
                }
                
                if (string.IsNullOrEmpty(cutscene.cutsceneId))
                {
                    Debug.LogWarning($"CutsceneManager: Cutscene {cutscene.name} has no ID set");
                    continue;
                }
                
                if (_cutsceneLookup.ContainsKey(cutscene.cutsceneId))
                {
                    Debug.LogWarning($"CutsceneManager: Duplicate cutscene ID {cutscene.cutsceneId}");
                    continue;
                }
                
                _cutsceneLookup[cutscene.cutsceneId] = cutscene;
            }
            
            Debug.Log($"CutsceneManager: Initialized with {_cutsceneLookup.Count} cutscenes");
        }

        private void InitializeExecutor()
        {
            // Try to get existing services
            var npcMoveService = ServiceLocater.GetService<IMemoryNpcMoveService>();
            var dialogueService = ServiceLocater.GetService<IMemoryDialogueService>();
            
            // If services don't exist, create wrapper implementations
            if (dialogueService == null)
            {
                Debug.LogWarning("CutsceneManager: IMemoryDialogueService not found, creating wrapper");
                var dialogueServiceObj = new GameObject("_CutsceneDialogueService");
                dialogueServiceObj.transform.SetParent(transform);
                dialogueService = dialogueServiceObj.AddComponent<MemoryDialogueService>();
            }
            
            if (npcMoveService == null)
            {
                Debug.LogWarning("CutsceneManager: IMemoryNpcMoveService not found, creating wrapper");
                var npcServiceObj = new GameObject("_CutsceneNpcMoveService");
                npcServiceObj.transform.SetParent(transform);
                npcMoveService = npcServiceObj.AddComponent<MemoryNpcMoveService>();
            }
            
            if (npcMoveService == null || dialogueService == null)
            {
                Debug.LogError("CutsceneManager: Failed to initialize required services");
                return;
            }
            
            var context = new MemoryActionContext(dialogueService, npcMoveService);
            _executor = new CutsceneExecutor(context);
            
            Debug.Log("CutsceneManager: Executor initialized");
        }

        private void HandlePlayCutsceneRequest(string cutsceneId)
        {
            PlayCutscene(cutsceneId);
        }
        
        public void PlayCutscene(string cutsceneId)
        {
            if (string.IsNullOrEmpty(cutsceneId))
            {
                Debug.LogError("CutsceneManager: Cannot play cutscene with null or empty ID");
                return;
            }

            if (!_cutsceneLookup.TryGetValue(cutsceneId, out var cutscene))
            {
                Debug.LogError($"CutsceneManager: Cutscene with ID {cutsceneId} not found");
                return;
            }

            if (_executor == null)
            {
                Debug.LogError("CutsceneManager: Executor not initialized");
                return;
            }

            if (_executor.IsPlaying)
            {
                Debug.LogWarning("CutsceneManager: A cutscene is already playing");
                return;
            }

            Debug.Log($"CutsceneManager: Playing cutscene {cutsceneId}");
            _ = _executor.PlayCutsceneAsync(cutscene);
        }
        
        public bool HasCutscene(string cutsceneId)
        {
            return _cutsceneLookup.ContainsKey(cutsceneId);
        }
        
        public CutsceneData GetCutscene(string cutsceneId)
        {
            _cutsceneLookup.TryGetValue(cutsceneId, out var cutscene);
            return cutscene;
        }
    }
}

