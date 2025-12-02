using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Memory;
using UnityEngine;

namespace _Project.Scripts.Debugs
{
    public class DebugJournalHotkeys : MonoBehaviour
    {
        [SerializeField] private MemoryDatabase memoryDatabase;

        private PlayerProfile _profile;

        private void Start()
        {
            _profile = ServiceLocater.GetService<PlayerProfile>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                foreach (var memory in memoryDatabase.GetAllMemories())
                {
                    _profile.AddUnlockedMemory(memory.memoryId);

                    var notesList = new List<Notes>();
                    foreach (var observation in memory.memoryObservations)
                    {
                        var notes = new Notes
                        {
                            ObservationId = observation.observationId,
                            MemoryId = memory.memoryId,
                            NoteText = observation.observationText,
                            CurrentState = ObservationState.Unknown
                        };
                        
                        notesList.Add(notes);
                    }
                    
                    _profile.AddMemoryNotes(memory.memoryId, notesList);
                    _profile.AddFragmentCompletedMemory(memory.memoryId);
                }

                Debug.Log("<color=yellow>[DEBUG] Notes populated from MemoryDatabase. </color>");
            }
        }
    }
}