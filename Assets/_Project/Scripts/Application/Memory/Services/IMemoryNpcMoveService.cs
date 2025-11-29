using _Project.Scripts.Data.Memory.Actions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Application.Memory.Services
{
    public interface IMemoryNpcMoveService
    {
        Task MoveAlongPathAsync(string npcId, BezierPath path, float speed);
    }
}