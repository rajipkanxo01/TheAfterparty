using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using UnityEngine;

namespace _Project.Scripts.Presentation.Memory.Services
{
    /// <summary>
    /// Manages sprite replacements in memory scenes.
    /// Used to swap sprites after dialogue events (e.g., Elliot removing his jacket).
    /// </summary>
    public class MemorySpriteManager : MonoBehaviour
    {
        [System.Serializable]
        public class SpriteReplacement
        {
            [Tooltip("The GameObject name or path to find the sprite renderer")]
            public string targetObjectName;
            
            [Tooltip("The sprite to replace with")]
            public Sprite replacementSprite;
            
            [Tooltip("Optional: specify a child renderer by name")]
            public string childRendererName;
        }
        
        [Header("Sprite Replacements")]
        [Tooltip("Define sprite replacements that can be triggered by dialogue commands")]
        public List<SpriteReplacement> spriteReplacements = new();
        
        private Dictionary<string, SpriteReplacement> _replacementMap;
        private Dictionary<string, Sprite> _originalSprites = new();
        
        private void Awake()
        {
            ServiceLocater.RegisterService(this);
            InitializeReplacementMap();
        }
        
        private void InitializeReplacementMap()
        {
            _replacementMap = new Dictionary<string, SpriteReplacement>();
            
            foreach (var replacement in spriteReplacements)
            {
                if (!string.IsNullOrEmpty(replacement.targetObjectName))
                {
                    _replacementMap[replacement.targetObjectName] = replacement;
                }
            }
            
            Debug.Log($"MemorySpriteManager: Initialized with {_replacementMap.Count} sprite replacements");
        }
        
        /// <summary>
        /// Replace a sprite by the target object name
        /// </summary>
        public void ReplaceSprite(string targetName)
        {
            if (!_replacementMap.TryGetValue(targetName, out var replacement))
            {
                Debug.LogWarning($"MemorySpriteManager: No sprite replacement found for '{targetName}'");
                return;
            }
            
            GameObject targetObject = GameObject.Find(replacement.targetObjectName);
            
            if (targetObject == null)
            {
                Debug.LogWarning($"MemorySpriteManager: GameObject '{replacement.targetObjectName}' not found in scene");
                return;
            }
            
            SpriteRenderer spriteRenderer = null;
            
            // If child renderer name is specified, find it
            if (!string.IsNullOrEmpty(replacement.childRendererName))
            {
                Transform child = targetObject.transform.Find(replacement.childRendererName);
                if (child != null)
                {
                    spriteRenderer = child.GetComponent<SpriteRenderer>();
                }
                else
                {
                    Debug.LogWarning($"MemorySpriteManager: Child '{replacement.childRendererName}' not found on '{targetName}'");
                }
            }
            else
            {
                spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
            }
            
            if (spriteRenderer == null)
            {
                Debug.LogWarning($"MemorySpriteManager: No SpriteRenderer found on '{targetName}'");
                return;
            }
            
            // Store original sprite if not already stored
            string key = GetSpriteKey(targetName, replacement.childRendererName);
            if (!_originalSprites.ContainsKey(key))
            {
                _originalSprites[key] = spriteRenderer.sprite;
                Debug.Log($"MemorySpriteManager: Stored original sprite for '{key}'");
            }
            
            // Replace the sprite
            spriteRenderer.sprite = replacement.replacementSprite;
            Debug.Log($"MemorySpriteManager: Replaced sprite for '{targetName}' -> '{replacement.replacementSprite.name}'");
        }
        
        /// <summary>
        /// Restore the original sprite
        /// </summary>
        public void RestoreSprite(string targetName)
        {
            if (!_replacementMap.TryGetValue(targetName, out var replacement))
            {
                Debug.LogWarning($"MemorySpriteManager: No sprite replacement found for '{targetName}'");
                return;
            }
            
            string key = GetSpriteKey(targetName, replacement.childRendererName);
            
            if (!_originalSprites.TryGetValue(key, out var originalSprite))
            {
                Debug.LogWarning($"MemorySpriteManager: No original sprite stored for '{key}'");
                return;
            }
            
            GameObject targetObject = GameObject.Find(replacement.targetObjectName);
            
            if (targetObject == null)
            {
                Debug.LogWarning($"MemorySpriteManager: GameObject '{replacement.targetObjectName}' not found in scene");
                return;
            }
            
            SpriteRenderer spriteRenderer = null;
            
            if (!string.IsNullOrEmpty(replacement.childRendererName))
            {
                Transform child = targetObject.transform.Find(replacement.childRendererName);
                if (child != null)
                {
                    spriteRenderer = child.GetComponent<SpriteRenderer>();
                }
            }
            else
            {
                spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
            }
            
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = originalSprite;
                Debug.Log($"MemorySpriteManager: Restored original sprite for '{targetName}'");
            }
        }
        
        /// <summary>
        /// Clear all stored original sprites (useful when exiting memory)
        /// </summary>
        public void ClearOriginalSprites()
        {
            _originalSprites.Clear();
            Debug.Log("MemorySpriteManager: Cleared all original sprites");
        }
        
        private string GetSpriteKey(string targetName, string childName)
        {
            return string.IsNullOrEmpty(childName) ? targetName : $"{targetName}/{childName}";
        }
        
        private void OnDestroy()
        {
            ClearOriginalSprites();
        }
    }
}

