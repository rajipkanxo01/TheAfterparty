using System;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using PixeLadder.SimpleTooltip;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal.NotesTab
{
    public class JournalNoteEntry : MonoBehaviour
    {
        private Button _button;
        private string _observationId;
        private string _memoryId;
        
        private PlayerProfile _playerProfile;
        private TooltipTrigger _tooltipTrigger;
        
        public void Init(string observationId, string memoryId)
        {
            _observationId = observationId;
            _memoryId = memoryId;

            _button = GetComponent<Button>();

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnClick);
            
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();

            // 🔹 Set up Easy Tooltip once – it will show on hover automatically
            SetupTooltip();
        }

        private void SetupTooltip()
        {
            if (_playerProfile == null)
                return;

            var note = _playerProfile.GetNote(_memoryId, _observationId);
            if (note == null)
                return;

            string tooltipContent = GetTooltipContent(note);
            string tooltipTitle = GetTooltipTitle(note);

            // If already has a trigger, just update it instead of adding a new one
            if (_tooltipTrigger == null)
            {
                _tooltipTrigger = TooltipTrigger.AddTooltip(gameObject, tooltipContent, tooltipTitle);
            }
            else
            {
                _tooltipTrigger.Title = tooltipTitle;
                _tooltipTrigger.Content = tooltipContent;
            }
            
            if (_tooltipTrigger != null)
            {
                _tooltipTrigger.TitleColor = GetColorForState(note.CurrentState);
                _tooltipTrigger.HoverDelay = 0.3f;
            }
        }
        
        private string GetTooltipTitle(Notes note)
        {
            switch (note.CurrentState)
            {
                case ObservationState.Verified:
                    return "Verified Observation";
                case ObservationState.Contradicted:
                    return "Contradicted Observation";
                case ObservationState.Unknown:
                default:
                    return "Unverified Observation";
            }
        }
        
        private string GetTooltipContent(Notes note)
        {
            string stateDescription = note.CurrentState switch
            {
                ObservationState.Verified =>
                    "This observation has been verified and is confirmed to be true.",
                ObservationState.Contradicted =>
                    "This observation has been contradicted by evidence found.",
                ObservationState.Unknown =>
                    "This observation has not yet been verified or contradicted.",
                _ => "Status unknown."
            };

            return $"{note.NoteText}\n\n<i>{stateDescription}</i>";
        }
        
        private Color GetColorForState(ObservationState state)
        {
            ColorUtility.TryParseHtmlString("#ECA116", out var contradicatedColor);
            ColorUtility.TryParseHtmlString("#53AD40", out var verifiedColor);
            
            return state switch
            {
                ObservationState.Verified      => verifiedColor,
                ObservationState.Contradicted  => contradicatedColor,
                ObservationState.Unknown       => new Color(0.8f, 0.8f, 0.8f),
                _                              => Color.white
            };
        }
        
        private void OnClick()
        {
            Debug.Log("JournalNoteEntry: Note clicked: " + _observationId + " for memory: " + _memoryId);
            var notes = _playerProfile.GetNote(_memoryId, _observationId);

            switch (notes.CurrentState)
            {
                case ObservationState.Unknown:
                    Debug.LogWarning($"Unknown note state: {notes.CurrentState}");
                    break;
                case ObservationState.Verified:
                    Debug.LogWarning($"Verified note state: {notes.CurrentState}");
                    break;
                case ObservationState.Contradicted:
                    Debug.LogWarning($"Contradicted note state: {notes.CurrentState}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnDisable()
        {
            TooltipManager.Instance?.HideTooltip();
        }
    }
}
