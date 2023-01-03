using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;

using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Utilities;

namespace HideChatPlugin
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Hide Chat Plugin";
        private BaseNode chatNode;

        private GameGui GuiManager { get; init; }
        public Framework FrameworkManager { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] GameGui guiManager,
            [RequiredVersion("1.0")] Framework framework
        )
        {
            this.GuiManager = guiManager;
            this.FrameworkManager = framework;

            this.chatNode = new BaseNode("ChatLog", this.GuiManager);
            this.FrameworkManager.Update += OnFramework;
        }

        public void Dispose()
        {
            this.FrameworkManager.Update -= OnFramework;

            unsafe
            {
                this.chatNode.GetRootNode()->ToggleVisibility(true);
            }
        }

        public void OnFramework(Framework framework)
        {
            unsafe
            {
                // IDs found using /xldata ai
                var visible = this.chatNode.GetComponentNode(5).GetNode<AtkResNode>(2)->IsVisible;
                this.chatNode.GetRootNode()->ToggleVisibility(visible);
            }
        }
    }
}
