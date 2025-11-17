using System;
using _Project.Scripts.Application.Player;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Application.Badge
{
    public static class BadgeEvents
    {
        public static event Action OnSpawnBadge;
        public static event Action OnBadgePicked;

        public static void RaiseSpawnBadge()
        {
            OnSpawnBadge?.Invoke();
        }

        public static void RaiseBadgePicked()
        {
            OnBadgePicked?.Invoke();
        }
    }

    public class BadgeManager
    {
        private readonly PlayerProfile _playerProfile;
        private readonly GameObject _badgePrefab;
        private readonly Vector3 _spawnPosition;
        private readonly Quaternion _spawnRotation;

        private const string BadgePickedKey = "detective_badge_picked";
        private bool _isSpawned;

        public BadgeManager(PlayerProfile profile, GameObject badgePrefab, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            _playerProfile = profile;
            _badgePrefab = badgePrefab;
            _spawnPosition = spawnPosition;
            _spawnRotation = spawnRotation;

            BadgeEvents.OnSpawnBadge += HandleSpawnBadge;
            BadgeEvents.OnBadgePicked += HandleBadgePicked;
        }

        private void HandleSpawnBadge()
        {
            if (_isSpawned || _playerProfile.HasFlag(BadgePickedKey))
            {
                Debug.Log($"BadgeManager: Spawn skipped. _isSpawned={_isSpawned}, AlreadyPicked={_playerProfile.HasFlag(BadgePickedKey)}");
                return;
            }

            if (_badgePrefab == null)
            {
                return;
            }
            
            var instance = Object.Instantiate(_badgePrefab, _spawnPosition, _spawnRotation);
            if (instance == null)
            {
                return;
            }

            _isSpawned = true;
        }

        private void HandleBadgePicked()
        {
            if (!_playerProfile.HasFlag(BadgePickedKey))
            {
                _playerProfile.SetFlag(BadgePickedKey, true);
                ToastNotification.Show("Badge picked up!!!");
            }
        }

        public void Cleanup()
        {
            BadgeEvents.OnSpawnBadge -= HandleSpawnBadge;
            BadgeEvents.OnBadgePicked -= HandleBadgePicked;
        }
    }
}