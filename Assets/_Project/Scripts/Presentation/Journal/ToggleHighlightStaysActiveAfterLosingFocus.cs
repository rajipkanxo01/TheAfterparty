using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal
{
    public class ToggleHighlightStaysActiveAfterLosingFocus : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image imageToKeepFocusActive;

        private void Reset()
        {
            toggle = GetComponent<Toggle>();
        }

        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        
        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
        
        private void OnToggleValueChanged(bool isOn)
        {
            if (imageToKeepFocusActive == null) return;
            
            imageToKeepFocusActive.color = toggle.isOn ? toggle.colors.highlightedColor : Color.clear;
        }
    }
}