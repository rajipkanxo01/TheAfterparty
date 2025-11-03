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
            RegisterCommand(new VisitMemoryCommandHandler());
            RegisterCommand(new CutsceneCommandHandler());
            RegisterCommand(new SceneCommandHandler());
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

                case "VisitMemory":
                    _runner.AddCommandHandler<string>(handler.CommandName, async (memoryId) =>
                    {
                        await handler.ExecuteAsync(memoryId);
                    });
                    break;
                
                case "cutscene":
                    _runner.AddCommandHandler<string[]>(handler.CommandName, async (args) =>
                    {
                        await handler.ExecuteAsync(args);
                    });
                    break;
                
                case "scene":
                    _runner.AddCommandHandler<string[]>(handler.CommandName, async (args) => {
                        await handler.ExecuteAsync(args);
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