using System;
using System.Collections;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Events;
using _Project.Scripts.Application.Memory.Events;
using PixeLadder.SimpleTooltip;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Presentation.Journal.NotesTab
{
    public class ContradictionCounter : MonoBehaviour
    {
        [SerializeField] private string memoryID;
        [SerializeField] private TextMeshProUGUI foundContradictionsText;
        [SerializeField] private TextMeshProUGUI totalNeededContradictionsText;
        [SerializeField] private float blinkSpeed = 4f;
        [SerializeField] private Color blinkColor = Color.yellow;
        
        private NotesYarnBridge _notesYarnBridge;
        private TooltipTrigger _tooltipTrigger;
        private Coroutine _blinkRoutine;
        private Color _originalColorFound;
        private Color _originalColorNeeded;
        private bool _isComplete;

        private void Start()
        {
            _notesYarnBridge = ServiceLocater.GetService<NotesYarnBridge>();
            
            if (_notesYarnBridge == null)
            {
                Debug.LogWarning("MemoryNoteButton: NotesYarnBridge not found in ServiceLocater, trying FindAnyObjectByType...");
                _notesYarnBridge = FindAnyObjectByType<NotesYarnBridge>();
                
                if (_notesYarnBridge == null)
                {
                    Debug.LogWarning("MemoryNoteButton: NotesYarnBridge not found. Contradiction counts will not be displayed.");
                }
                else
                {
                    Debug.Log("MemoryNoteButton: NotesYarnBridge found via FindAnyObjectByType.");
                }
            }
            
            // Store original text colors for blinking
            if (foundContradictionsText != null)
            {
                _originalColorFound = foundContradictionsText.color;
            }
            if (totalNeededContradictionsText != null)
            {
                _originalColorNeeded = totalNeededContradictionsText.color;
            }
        }

        public void UpdateContradictionCounts()
        {
            // Read contradiction counts from Yarn variables
            float foundCount = _notesYarnBridge.GetVariableValue($"${memoryID}_contradictions_found", 0f);
            float neededCount = _notesYarnBridge.GetVariableValue($"${memoryID}_contradictions_needed", 0f);
            
            Debug.Log($"ContradictionCounter: For memory '{memoryID}', found contradictions: {foundCount}, needed contradictions: {neededCount}");

            // Update UI text elements if they exist
            if (foundContradictionsText != null)
            {
                foundContradictionsText.text = foundCount.ToString("0") + "/";
            }

            if (totalNeededContradictionsText != null)
            {
                totalNeededContradictionsText.text = neededCount.ToString("0");
            }
            
            // Check if we've reached the needed contradictions
            bool isComplete = foundCount >= neededCount && neededCount > 0;
            
            if (isComplete && !_isComplete)
            {
                // Just became complete - start blinking and show tooltip
                
                UIEvents.RaiseAllContradictionsFound(memoryID);
                _isComplete = true;
                StartBlinking();
                SetupTooltip("All contradictions found. Go and present these contradictions to Elliot to restore this memory.");
                
            }
            else if (!isComplete && _isComplete)
            {
                // No longer complete - stop blinking and remove tooltip
                _isComplete = false;
                StopBlinking();
                RemoveTooltip();
            }
        }
        
        public void ClearUI()
        {
            if (foundContradictionsText != null)
            {
                foundContradictionsText.text = "";
            }
            if (totalNeededContradictionsText != null)
            {
                totalNeededContradictionsText.text = "";
            }
            
            StopBlinking();
            RemoveTooltip();
            _isComplete = false;
        }
        
        private void StartBlinking()
        {
            if (_blinkRoutine != null)
            {
                StopCoroutine(_blinkRoutine);
            }
            _blinkRoutine = StartCoroutine(BlinkRoutine());
        }
        
        private void StopBlinking()
        {
            if (_blinkRoutine != null)
            {
                StopCoroutine(_blinkRoutine);
                _blinkRoutine = null;
            }
            
            // Reset to original colors
            if (foundContradictionsText != null)
            {
                foundContradictionsText.color = _originalColorFound;
            }
            if (totalNeededContradictionsText != null)
            {
                totalNeededContradictionsText.color = _originalColorNeeded;
            }
        }
        
        private IEnumerator BlinkRoutine()
        {
            float elapsed = 0f;
            while (true)
            {
                float t = (Mathf.Sin(elapsed * blinkSpeed) + 1f) / 2f;
                
                if (foundContradictionsText != null)
                {
                    foundContradictionsText.color = Color.Lerp(_originalColorFound, blinkColor, t);
                }
                if (totalNeededContradictionsText != null)
                {
                    totalNeededContradictionsText.color = Color.Lerp(_originalColorNeeded, blinkColor, t);
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        private void SetupTooltip(string message)
        {
            if (_tooltipTrigger == null)
            {
                _tooltipTrigger = TooltipTrigger.AddTooltip(gameObject, message);
            }
            else
            {
                _tooltipTrigger.Content = message;
            }
        }
        
        private void RemoveTooltip()
        {
            if (_tooltipTrigger != null)
            {
                Destroy(_tooltipTrigger);
                _tooltipTrigger = null;
            }
        }

        private void OnDisable()
        {
            RemoveTooltip();
        }
    }
}