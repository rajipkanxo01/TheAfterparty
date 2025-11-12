using System;
using _Project.Scripts.Application.Badge;
using _Project.Scripts.Application.Core;
using UnityEngine;
using Yarn.Unity;

namespace _Project.Scripts.Application.Dialogue
{
    public class DetectiveStateListener : MonoBehaviour
    {
        [SerializeField] private DialogueRunner dialogueRunner;

        private void Awake()
        {
            ServiceLocater.RegisterService(this);
        }

        private void Start()
        {
            BadgeEvents.OnBadgePicked += HandleBadgePicked;
        }

        private void OnDestroy()
        {
            BadgeEvents.OnBadgePicked -= HandleBadgePicked;
        }

        private void HandleBadgePicked()
        {
            if (dialogueRunner == null)
            {
                Debug.LogWarning("[DetectiveStateListener] DialogueRunner not found.");
                return;
            }

            dialogueRunner.VariableStorage.SetValue("$detective_state", "badge_returned");
        }
    }
}