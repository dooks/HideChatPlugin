using System.Collections.Generic;
using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Logging;

using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Utilities;

namespace HideChatPlugin
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Hide Chat Plugin";
        private const string CommandName = "/togglehidechat";
        private bool forcedVisibility { get; set; } = false;
        private List<BaseNode> chatNodes = new();

        private List<string> chatNodeIds = new()
        {
            "ChatLog",
            "ChatLogPanel_0",
            "ChatLogPanel_1",
            "ChatLogPanel_2",
            "ChatLogPanel_3"
        };

        private CommandManager CommandManager { get; init; }
        private GameGui GameGui { get; init; }
        private Framework FrameworkManager { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] CommandManager commandManager,
            [RequiredVersion("1.0")] GameGui gameGui,
            [RequiredVersion("1.0")] Framework framework
        )
        {
            this.CommandManager = commandManager;
            this.GameGui = gameGui;
            this.FrameworkManager = framework;

            this.FrameworkManager.Update += OnFramework;
            this.CommandManager.AddHandler(CommandName, new CommandInfo(ToggleForcedVisibility)
            {
                HelpMessage = "Toggle forced visibility of chat."
            });

            foreach (string id in chatNodeIds)
            {
                chatNodes.Add(new BaseNode(id, this.GameGui));
            }
        }

        public unsafe void ToggleVisibility(bool visibility)
        {
            this.chatNodes.ForEach(delegate (BaseNode node)
            {
                node.GetRootNode()->ToggleVisibility(visibility);
            });
        }
        
        public void ToggleForcedVisibility(string command, string args)
        {
            this.forcedVisibility = !this.forcedVisibility;
        }

        public void Dispose()
        {
            this.FrameworkManager.Update -= OnFramework;

            ToggleVisibility(true);
            this.chatNodes.Clear();
            this.forcedVisibility = false;
        }

        public void OnFramework(Framework framework)
        {
            var chatLogNode = this.chatNodes[0];
            unsafe
            {
                // IDs found using /xldata ai
                var visible = this.forcedVisibility || chatLogNode.GetComponentNode(5).GetNode<AtkResNode>(2)->IsVisible;
                ToggleVisibility(visible);
            }
        }
    }
}
