using System.Threading.Tasks;

namespace _Project.Scripts.Application.Memory.Services
{
    public interface IMemoryDialogueService
    {
        Task PlayNodeAsync(string nodeName);
    }
}