using System.Threading.Tasks;
using _Project.Scripts.Data.Memory.Actions;

namespace _Project.Scripts.Application.Memory.Actions
{
    public interface IMemoryActionExecutor
    {
        bool CanExecute(ActionBaseData actionData);
        Task ExecuteAsync(ActionBaseData actionData, MemoryActionContext context);
    }
}