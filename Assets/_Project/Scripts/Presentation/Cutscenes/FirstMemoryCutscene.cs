using System;
using System.Collections;
using _Project.Scripts.Presentation.Npc;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Presentation.Dialogue
{
    public class FirstMemoryCutscene : MonoBehaviour, ICutscene
    {
        [SerializeField] private NpcCutscene detective;
        [SerializeField] private NpcCutscene girl;
        [SerializeField] private NpcCutscene mascot;

        private enum MemoryState
        {
            DetectiveGirlTalking,
            GirlRunning,
            ElliotChasingGirl,
            ElliotChasingMascot,
            MascotLeavesAndGirlReturns,
            GirlPlacesBadge,
            GirlLeaves
        }

        private MemoryState state = MemoryState.DetectiveGirlTalking;
        public bool IsPlaying { get; private set; }

        private void Update()
        {
            // TEMP: bound to space for testing
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                NextAnimation();
            }
        }

        public void NextAnimation()
        {
            IsPlaying = true;

            Debug.Log($"FirstMemoryCutscene: Starting cutscene animation {state}");

            switch (state)
            {
                case MemoryState.DetectiveGirlTalking:
                    state = MemoryState.GirlRunning;

                    girl.NextPosition(2);
                    break;
                case MemoryState.GirlRunning:
                    
                    state = MemoryState.ElliotChasingGirl;
                    detective.NextPosition();
                    girl.NextPosition(2);
                    mascot.NextPosition();
                    break;
                case MemoryState.ElliotChasingGirl:
                    state = MemoryState.ElliotChasingMascot;
                    mascot.NextPosition();
                    detective.NextPosition(3);
                    break;
                case MemoryState.ElliotChasingMascot:
                    state = MemoryState.MascotLeavesAndGirlReturns;
                    mascot.NextPosition();
                    girl.NextPosition(1.5f);
                    break;
                case MemoryState.MascotLeavesAndGirlReturns:
                    state = MemoryState.GirlPlacesBadge;
                    girl.NextPosition(1.5f);
                    break;
                case MemoryState.GirlPlacesBadge:
                    state = MemoryState.GirlLeaves;
                    girl.NextPosition(1.5f);
                    break;
            }

            StartCoroutine(WaitUntilAllNpcStop());
        }

        private IEnumerator WaitUntilAllNpcStop()
        {
            var npcs = new[] { detective, girl, mascot };
            bool moving;
            do
            {
                moving = false;
                foreach (var npc in npcs)
                {
                    if (npc != null && npc.IsMoving)
                    {
                        moving = true;
                        break;
                    }
                }

                yield return null;
            } while (moving);

            IsPlaying = false;
        }
    }
}