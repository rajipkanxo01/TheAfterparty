using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yarn.Unity;
using _Project.Scripts.Application.Dialogue.Commands;

namespace _Project.Scripts.Application.Dialogue
{
    public class DialogueCommandRegistry
    {
        private readonly DialogueRunner _runner;
        private readonly List<IDialogueCommandHandler> _handlers = new();

        public DialogueCommandRegistry(DialogueRunner runner)
        {
            _runner = runner;
        }
        
        public void RegisterBuiltInCommands()
        {
            RegisterCommand(new SayCommandHandler());
            RegisterCommand(new MemoryCommandHandler());
            RegisterCommand(new BadgeCommandHandler());
        }

        private void RegisterCommand(IDialogueCommandHandler handler)
        {
            if (handler == null)
                return;

            _handlers.Add(handler);

            switch (handler.CommandName)
            {
                case "say":
                    _runner.AddCommandHandler<string, string>(handler.CommandName, async (speaker, line) =>
                    {
                        await handler.ExecuteAsync(speaker, line);
                    });
                    break;
                
                case "memory":
                    _runner.AddCommandHandler<string, string>(handler.CommandName, async (mode, sceneId) =>
                    {
                        await handler.ExecuteAsync(mode, sceneId);
                    });
                    break;
                
                case "spawn_badge":
                    _runner.AddCommandHandler(handler.CommandName, async () =>
                    {
                        await handler.ExecuteAsync();
                    });
                    break;

                default:
                    _runner.AddCommandHandler(handler.CommandName, async () =>
                    {
                        await handler.ExecuteAsync();
                    });
                    break;
            }
        }
        
       
    }
}