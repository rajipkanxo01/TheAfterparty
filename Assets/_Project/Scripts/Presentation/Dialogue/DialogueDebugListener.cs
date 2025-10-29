using _Project.Scripts.Application.Dialogue;
using UnityEngine;

namespace _Project.Scripts.Presentation.Dialogue
{
    public class DialogueDebugListener : MonoBehaviour
    {
        [SerializeField] private DialogueController controller;

        private void OnEnable()
        {
            controller.OnDialogueStarted += OnStart;
            controller.OnDialogueEnded += OnEnd;
            controller.OnDialogueLineStarted += OnLine;
        }

        private void OnDisable()
        {
            controller.OnDialogueStarted -= OnStart;
            controller.OnDialogueEnded -= OnEnd;
            controller.OnDialogueLineStarted -= OnLine;
        }

        private void OnStart()
        {
            Debug.Log("Dialogue started.");
        }

        private void OnEnd()
        {
            Debug.Log("Dialogue ended.");
        }

        private void OnLine(object sender, DialogueLineEventArgs e)
        {
            Debug.Log($"[{e.SpeakerName}] {e.LineText}");
        }
    }
}