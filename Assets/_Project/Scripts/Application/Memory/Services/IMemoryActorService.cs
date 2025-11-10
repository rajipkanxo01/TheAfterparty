using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Application.Memory.Services
{
    public interface IMemoryActorService
    {
        Task MoveAlongPathAsync(string actorId, IReadOnlyList<Vector3> positions, float speed);
    }
}