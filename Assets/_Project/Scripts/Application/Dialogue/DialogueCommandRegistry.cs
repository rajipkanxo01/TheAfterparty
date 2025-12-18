﻿using System;
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
            RegisterCommand(new SetHighlightAreaCommandHandler());
            RegisterCommand(new RaiseEventCommandHandler());
            RegisterCommand(new NotesCommandHandler());
            RegisterCommand(new LoadSceneCommandHandler());
            RegisterCommand(new ReplaceSpriteCommandHandler());
        }

        private void RegisterCommand(IDialogueCommandHandler handler)
        {
            if (handler == null)
                return;

            _handlers.Add(handler);

            switch (handler.CommandName)
            {
                case "say":
                    _runner.AddCommandHandler<string, string>(handler.CommandName,
                        async (speaker, line) => { await handler.ExecuteAsync(speaker, line); });
                    break;

                case "memory":
                    _runner.AddCommandHandler<string, string>(handler.CommandName,
                        async (mode, sceneId) => { await handler.ExecuteAsync(mode, sceneId); });
                    break;

                case "spawn_badge":
                    _runner.AddCommandHandler(handler.CommandName, async () => { await handler.ExecuteAsync(); });
                    break;

                case "set_active_point":
                    _runner.AddCommandHandler<string>(handler.CommandName,
                        async (activePoint) => { await handler.ExecuteAsync(activePoint); });
                    break;

                case "raise_event":
                    _runner.AddCommandHandler<string>(handler.CommandName,
                        async (code) => { await handler.ExecuteAsync(code); });
                    break;
                
                case "note":
                    _runner.AddCommandHandler<string, string, string>(handler.CommandName,
                        async (state, memoryId, observationId) => { await handler.ExecuteAsync(state, memoryId, observationId); });
                    break;
                
                case "load_scene":
                    _runner.AddCommandHandler<string>(handler.CommandName,
                        async (sceneName) => { await handler.ExecuteAsync(sceneName); });
                    break;
                
                case "replace_sprite":
                    _runner.AddCommandHandler<string>(handler.CommandName,
                        async (targetName) => { await handler.ExecuteAsync(targetName); });
                    break;

                default:
                    _runner.AddCommandHandler(handler.CommandName, async () => { await handler.ExecuteAsync(); });
                    break;
            }
        }
    }
}