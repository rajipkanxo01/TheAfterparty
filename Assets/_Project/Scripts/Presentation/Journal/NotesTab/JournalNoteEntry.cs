using System;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Events;
using _Project.Scripts.Application.Player;
using PixeLadder.SimpleTooltip;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal.NotesTab
{
    public class JournalNoteEntry : MonoBehaviour
    {
        [Header("Checkbox")]
        [SerializeField] private Image checkBoxIcon;
        [SerializeField] private Sprite tickSprite;
        [SerializeField] private Sprite xSprite;

        private Button _button;
        private string _observationId;
        private string _memoryId;

        private bool _allContradictionsFound;
        private PlayerProfile _playerProfile;
        private TooltipTrigger _tooltipTrigger;

        private void OnEnable()
        {
            UIEvents.OnAllContradictionsFound += HandleAllContradictionsFound;
        }

        private void HandleAllContradictionsFound(string memoryId)
        {
            if (memoryId == _memoryId)
            {
                _allContradictionsFound = true;

                var note = _playerProfile.GetNote(_memoryId, _observationId);
                
                Debug.Log("JournalNoteEntry: All contradictions found for memory " + memoryId + ". Updating entry for observation " + _observationId);
                
                ConfigureVisualState(note);
                SetupOrUpdateTooltip(note);
            }
        }

        public void Init(string observationId, string memoryId)
        {
            _observationId = observationId;
            _memoryId = memoryId;

            if (_button == null)
            {
                _button = GetComponent<Button>();
                if (_button == null)
                {
                    Debug.LogError("JournalNoteEntry: Button component not found.");
                    return;
                }
            }

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnClick);

            if (checkBoxIcon != null)
            {
                checkBoxIcon.enabled = false;
                checkBoxIcon.sprite = null;
            }

            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            if (_playerProfile == null)
            {
                Debug.LogError("JournalNoteEntry: PlayerProfile not found in ServiceLocater.");
                _button.interactable = false;
                return;
            }

            var note = _playerProfile.GetNote(_memoryId, _observationId);
            if (note == null)
            {
                Debug.LogWarning($"JournalNoteEntry: No note found for memory '{_memoryId}' and observation '{_observationId}'.");
                _button.interactable = false;
                return;
            }

            // Check once when the entry is created
            _allContradictionsFound = _playerProfile.HasFoundAllContradictions(_memoryId);

            ConfigureVisualState(note);
            SetupOrUpdateTooltip(note);
        }

        private void ConfigureVisualState(Notes note)
        {
            if (_button == null || checkBoxIcon == null)
                return;

            // Before all contradictions are found
            if (!_allContradictionsFound)
            {
                checkBoxIcon.enabled = false;
                checkBoxIcon.sprite = null;
                return;
            }

            // After all contradictions are found:
            bool isSelected = _playerProfile.IsContradictionSelected(_memoryId, _observationId);

            if (note.CurrentState == ObservationState.Contradicted)
            {
                _button.interactable = true;

                if (isSelected)
                {
                    checkBoxIcon.enabled = true;
                    if (tickSprite != null)
                    {
                        checkBoxIcon.sprite = tickSprite;
                    }
                }
                else
                {
                    checkBoxIcon.enabled = false;
                    checkBoxIcon.sprite = null;
                }
            }
            else
            {
                _button.interactable = false;
                checkBoxIcon.enabled = true;

                checkBoxIcon.sprite = xSprite != null ? xSprite : null;
            }
        }

        private void SetupOrUpdateTooltip(Notes note)
        {
            if (note == null)
                return;

            string tooltipTitle = GetTooltipTitle(note);
            string tooltipContent = GetTooltipContent(note);

            if (_tooltipTrigger == null)
            {
                Debug.Log("JournalNoteEntry: Adding tooltip trigger.");
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
            if (_allContradictionsFound && note.CurrentState == ObservationState.Contradicted)
            {
                bool isSelected = _playerProfile.IsContradictionSelected(_memoryId, _observationId);

                return isSelected
                    ? "Selected for Presentation"
                    : "Click to Select for Presentation";
            }

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
            if (note == null)
                return string.Empty;

            if (_allContradictionsFound && note.CurrentState == ObservationState.Contradicted)
            {
                bool isSelected = _playerProfile.IsContradictionSelected(_memoryId, _observationId);
                string actionText = isSelected
                    ? "Click again to deselect this contradiction."
                    : "Select this contradiction to present it to Elliot.";

                return $"{note.NoteText}\n\n<i>This contradiction can now be presented. {actionText}</i>";
            }

            string stateDescription;

            switch (note.CurrentState)
            {
                case ObservationState.Verified:
                    stateDescription = "This observation has been verified and is confirmed to be true.";
                    break;
                case ObservationState.Contradicted:
                    stateDescription = "This observation has been contradicted by evidence found.";
                    break;
                case ObservationState.Unknown:
                    stateDescription = "This observation has not yet been verified or contradicted.";
                    break;
                default:
                    stateDescription = "Status unknown.";
                    break;
            }

            return $"{note.NoteText}\n\n<i>{stateDescription}</i>";
        }

        private Color GetColorForState(ObservationState state)
        {
            ColorUtility.TryParseHtmlString("#ECA116", out var contradictedColor);
            ColorUtility.TryParseHtmlString("#53AD40", out var verifiedColor);

            return state switch
            {
                ObservationState.Verified => verifiedColor,
                ObservationState.Contradicted => contradictedColor,
                ObservationState.Unknown => new Color(0.8f, 0.8f, 0.8f),
                _ => Color.white
            };
        }


        private void OnClick()
        {
            if (_playerProfile == null)
                return;

            var note = _playerProfile.GetNote(_memoryId, _observationId);
            if (note == null)
            {
                Debug.LogWarning($"JournalNoteEntry: Clicked, but note not found for memory '{_memoryId}', observation '{_observationId}'.");
                return;
            }


            if (_allContradictionsFound && note.CurrentState == ObservationState.Contradicted)
            {
                _playerProfile.ToggleSelectedContradiction(_memoryId, _observationId);
                bool isNowSelected = _playerProfile.IsContradictionSelected(_memoryId, _observationId);

                if (checkBoxIcon != null)
                {
                    if (isNowSelected)
                    {
                        checkBoxIcon.enabled = true;
                        if (tickSprite != null)
                        {
                            checkBoxIcon.sprite = tickSprite;
                        }
                    }
                    else
                    {
                        //no icon for unselected contradictions
                        checkBoxIcon.enabled = false;
                        checkBoxIcon.sprite = null;
                    }
                }

                SetupOrUpdateTooltip(note);
                return;
            }

            switch (note.CurrentState)
            {
                case ObservationState.Unknown:
                    Debug.LogWarning($"JournalNoteEntry: Clicked UNKNOWN note ({_observationId}) in memory {_memoryId}.");
                    break;
                case ObservationState.Verified:
                    Debug.LogWarning($"JournalNoteEntry: Clicked VERIFIED note ({_observationId}) in memory {_memoryId}.");
                    break;
                case ObservationState.Contradicted:
                    Debug.LogWarning($"JournalNoteEntry: Clicked CONTRADICTED note ({_observationId}) in memory {_memoryId}, but contradictions are not all found yet.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void OnDisable()
        {
            UIEvents.OnAllContradictionsFound -= HandleAllContradictionsFound;
            TooltipManager.Instance?.HideTooltip();
        }
    }
}
