using System.Linq;
using _Project.Scripts.Data.Memory.Actions;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Application.Utilities
{
    public static class FragmentNpcUtility
    {
        public static bool ContainsNpc(FragmentData fragment, string npcId)
        {
            if (fragment == null || string.IsNullOrEmpty(npcId))
            {
                return false;
            }

            return fragment.realMemoryActions.Any(a => a.GetNpcId().Equals(npcId))
                   || fragment.corruptedMemoryActions.Any(a => a.GetNpcId().Equals(npcId));
        }

        
        public static Vector3 GetStartingPosition(FragmentData fragment, string npcId)
        {
            var move = GetMoveAction(fragment, npcId);

            if (move != null && move.pathPointNames.Count > 0)
            {
                var positions = MemoryPathUtility.ResolvePathPositions(move.pathPointNames);
                if (positions.Count > 0)
                {
                    return positions[0];
                }
            }

            return default;
        }

        
        private static MoveActionBaseData GetMoveAction(FragmentData fragment, string npcId)
        {
            if (fragment == null || string.IsNullOrEmpty(npcId))
                return null;

            return fragment.realMemoryActions
                       .OfType<MoveActionBaseData>()
                       .FirstOrDefault(a => a.npcId == npcId)
                   ??
                   fragment.corruptedMemoryActions
                       .OfType<MoveActionBaseData>()
                       .FirstOrDefault(a => a.npcId == npcId);
        }
        
    }
}
