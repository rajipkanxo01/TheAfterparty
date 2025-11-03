using System.Threading.Tasks;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public interface IDialogueCommandHandler
    {
        string CommandName { get; }
        Task ExecuteAsync(params string[] parameters);
    }
}