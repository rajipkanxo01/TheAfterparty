using System.Linq;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Memory.Actions;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Application.Utilities
{
    public static class FragmentNpcUtility
    {
        private static readonly PlayerProfile PlayerProfile;
        static FragmentNpcUtility()
        {
            PlayerProfile = ServiceLocater.GetService<PlayerProfile>();
        }

        public static bool ContainsNpc(FragmentData fragment, string npcId)
        {
            if (fragment == null || string.IsNullOrEmpty(npcId))
            {
                return false;
            }

            bool isRepaired = PlayerProfile.HasRepairedFragment(fragment.fragmentId);

            // if repaired → use REAL actions ONLY
            if (isRepaired)
            {
                return fragment.realMemoryActions.Any(a => a.GetNpcId() != null && a.GetNpcId() == npcId);
            }

            // if corrupted → use corrupted actions if available
            if (fragment.isCorrupted && fragment.HasCorruptedVersion)
            {
                return fragment.corruptedMemoryActions.Any(a => a.GetNpcId() != null && a.GetNpcId() == npcId);
            }

            return fragment.realMemoryActions.Any(a => a.GetNpcId() != null && a.GetNpcId() == npcId);
        }

        
        public static Vector3 GetStartingPosition(FragmentData fragment, string npcId)
        {
            var move = GetMoveAction(fragment, npcId);

            if (move != null && move.path != null)
            {
                return move.path.endPoint; // TODO: not sure if this should be the start...?
            }

            return default;
        }

        
        private static BezierMoveActionBaseData GetMoveAction(FragmentData fragment, string npcId)
        {
            if (fragment == null || string.IsNullOrEmpty(npcId))
                return null;

            return fragment.realMemoryActions
                       .OfType<BezierMoveActionBaseData>()
                       .FirstOrDefault(a => a.npcId == npcId)
                   ??
                   fragment.corruptedMemoryActions
                       .OfType<BezierMoveActionBaseData>()
                       .FirstOrDefault(a => a.npcId == npcId);
        }
        
    }
}
