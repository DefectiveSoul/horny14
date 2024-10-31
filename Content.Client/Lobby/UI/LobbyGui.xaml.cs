using Content.Client.Message;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client.AutoGenerated;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System.Numerics;
using Content.Client.Parallax.Managers;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Shared._Sunrise.SunriseCCVars;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Configuration;
using Robust.Shared.Input;

namespace Content.Client.Lobby.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class LobbyGui : UIScreen
    {
        [Dependency] private readonly IClientConsoleHost _consoleHost = default!;
        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly IParallaxManager _parallaxManager = default!;
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly IResourceCache _resourceCache = default!;
        [Dependency] private readonly IConfigurationManager _configurationManager = default!;

        public string LobbyParallax = "FastSpace"; // Sunrise-edit
        public bool ShowParallax; // Sunrise-edit
        private string _serverName = string.Empty;
        [ViewVariables(VVAccess.ReadWrite)] public Vector2 Offset { get; set; } // Sunrise-edit
        public const string DefaultIconExpanded = "/Textures/Interface/Nano/inverted_triangle.svg.png";
        public const string DefaultIconCollapsed = "/Textures/Interface/Nano/top_triangle.svg.png";
        public const string StylePropertyIconExpanded = "IconExpanded";
        public const string StylePropertyIconCollapsed = "IconCollapsed";

        public Texture? IconExpanded;
        public Texture? IconCollapsed;

        private readonly StyleBoxTexture _back;

        public LobbyGui()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);
            SetAnchorPreset(MainContainer, LayoutPreset.Wide);
            SetAnchorPreset(LobbyArt, LayoutPreset.Wide); // Sunrise-Edit

            LobbySong.SetMarkup(Loc.GetString("lobby-state-song-no-song-text"));

            LeaveButton.OnPressed += _ => _consoleHost.ExecuteCommand("disconnect");
            OptionsButton.OnPressed += _ => UserInterfaceManager.GetUIController<OptionsUIController>().ToggleWindow();

            // Sunrise-start
            ChatHider.OnKeyBindUp += args =>
            {
                if (args.Function != EngineKeyFunctions.Use)
                    return;

                ChatContent.Visible = !ChatContent.Visible;
                ChatHider.Texture = ChatContent.Visible ? IconExpanded : IconCollapsed;
            };

            ServerInfoHider.OnKeyBindUp += args =>
            {
                if (args.Function != EngineKeyFunctions.Use)
                    return;

                ServerInfoContent.Visible = !ServerInfoContent.Visible;
                ServerInfoHider.Texture = ServerInfoContent.Visible ? IconExpanded : IconCollapsed;
            };

            CharacterInfoHider.OnKeyBindUp += args =>
            {
                if (args.Function != EngineKeyFunctions.Use)
                    return;

                CharacterInfoContent.Visible = !CharacterInfoContent.Visible;
                CharacterInfoHider.Texture = CharacterInfoContent.Visible ? IconExpanded : IconCollapsed;
            };

            UserProfileHider.OnKeyBindUp += args =>
            {
                if (args.Function != EngineKeyFunctions.Use)
                    return;

                UserProfileContent.Visible = !UserProfileContent.Visible;
                UserProfileHider.Texture = UserProfileContent.Visible ? IconExpanded : IconCollapsed;
            };

            ServersHubHider.OnKeyBindUp += args =>
            {
                if (args.Function != EngineKeyFunctions.Use)
                    return;

                ServersHubContent.Visible = !ServersHubContent.Visible;
                ServersHubHider.Texture = ServersHubContent.Visible ? IconExpanded : IconCollapsed;
            };

            ChangelogHider.OnKeyBindUp += args =>
            {
                if (args.Function != EngineKeyFunctions.Use)
                    return;

                ChangelogContent.Visible = !ChangelogContent.Visible;
                ChangelogHider.Texture = ChangelogContent.Visible ? IconExpanded : IconCollapsed;
            };

            Offset = new Vector2(_random.Next(0, 1000), _random.Next(0, 1000));
            RectClipContent = true;

            var panelTex = _resourceCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
            _back = new StyleBoxTexture
            {
                Texture = panelTex,
                Modulate = new Color(37, 37, 42),
            };
            _back.SetPatchMargin(StyleBox.Margin.All, 10);

            LeftTopPanel.PanelOverride = _back;

            RightPanel.PanelOverride = _back;

            LeftBottomPanel.PanelOverride = _back;

            LeftTopPanel.PanelOverride = _back;

            LobbySongPanel.PanelOverride = _back;

            _configurationManager.OnValueChanged(SunriseCCVars.LobbyOpacity, OnLobbyOpacityChanged);
            _configurationManager.OnValueChanged(SunriseCCVars.ServersHubEnable, OnServersHubEnableChanged);
            _configurationManager.OnValueChanged(SunriseCCVars.InfoLinksDonate, OnServerNameChanged, true);

            SetLobbyOpacity(_configurationManager.GetCVar(SunriseCCVars.LobbyOpacity));
            SetServersHubEnable(_configurationManager.GetCVar(SunriseCCVars.ServersHubEnable));

            Chat.SetChatOpacity();

            ServerName.Text = Loc.GetString("ui-lobby-welcome", ("name", _serverName));
            LoadIcons();
            // Sunrise-end
        }

        // Sunrise-Start
        private void OnServerNameChanged(string serverName)
        {
            ServerName.Text = Loc.GetString("ui-lobby-welcome", ("name", serverName));
            _serverName = serverName;
        }

        private void LoadIcons()
        {
            if (!TryGetStyleProperty(StylePropertyIconExpanded, out IconExpanded))
                IconExpanded = _resourceCache.GetTexture(DefaultIconExpanded);

            if (!TryGetStyleProperty(StylePropertyIconCollapsed, out IconCollapsed))
            {
                IconCollapsed = _resourceCache.GetTexture(DefaultIconCollapsed);
            }

            ServersHubHider.Texture = ServersHubContent.Visible ? IconExpanded : IconCollapsed;
            ChangelogHider.Texture = ChangelogContent.Visible ? IconExpanded : IconCollapsed;
            ServerInfoHider.Texture = ServerInfoContent.Visible ? IconExpanded : IconCollapsed;
            CharacterInfoHider.Texture = CharacterInfoContent.Visible ? IconExpanded : IconCollapsed;
            ChatHider.Texture = ChatContent.Visible ? IconExpanded : IconCollapsed;
            UserProfileHider.Texture = CharacterInfoContent.Visible ? IconExpanded : IconCollapsed;
            ServersHubHider.Modulate = StyleNano.NanoGold;
            ChangelogHider.Modulate = StyleNano.NanoGold;
            ServerInfoHider.Modulate = StyleNano.NanoGold;
            CharacterInfoHider.Modulate = StyleNano.NanoGold;
            ChatHider.Modulate = StyleNano.NanoGold;
            UserProfileHider.Modulate = StyleNano.NanoGold;
        }

        private void OnServersHubEnableChanged(bool enable)
        {
            SetServersHubEnable(enable);
        }

        private void SetServersHubEnable(bool enable)
        {
            ServersHubBox.Visible = enable;
        }
        // Sunrise-End

        private void OnLobbyOpacityChanged(float opacity)
        {
            SetLobbyOpacity(opacity);
        }

        private void SetLobbyOpacity(float opacity)
        {
            _back.Modulate = new Color(37, 37, 42).WithAlpha(opacity);
        }
        // Sunrise-End

        public void SwitchState(LobbyGuiState state)
        {
            DefaultState.Visible = false;
            CharacterSetupState.Visible = false;

            switch (state)
            {
                case LobbyGuiState.Default:
                    DefaultState.Visible = true;
                    break;
                case LobbyGuiState.CharacterSetup:
                    CharacterSetupState.Visible = true;

                    UserInterfaceManager.GetUIController<LobbyUIController>().ReloadCharacterSetup();

                    break;
            }
        }

        // Sunrise-start
        protected override void Draw(DrawingHandleScreen handle)
        {
            if (!ShowParallax)
                return;

            foreach (var layer in _parallaxManager.GetParallaxLayers(LobbyParallax))
            {
                var tex = layer.Texture;
                var texSize = new Vector2i(
                    (tex.Size.X * (int)Size.X * 1) / 1920,
                    (tex.Size.Y * (int)Size.X * 1) / 1920
                );
                var ourSize = PixelSize;

                var currentTime = (float) _timing.RealTime.TotalSeconds;
                var offset = Offset + new Vector2(currentTime * 100f, currentTime * 0f);

                if (layer.Config.Tiled)
                {
                    // Multiply offset by slowness to match normal parallax
                    var scaledOffset = (offset * layer.Config.Slowness).Floored();

                    // Then modulo the scaled offset by the size to prevent drawing a bunch of offscreen tiles for really small images.
                    scaledOffset.X %= texSize.X;
                    scaledOffset.Y %= texSize.Y;

                    // Note: scaledOffset must never be below 0 or there will be visual issues.
                    // It could be allowed to be >= texSize on a given axis but that would be wasteful.

                    for (var x = -scaledOffset.X; x < ourSize.X; x += texSize.X)
                    {
                        for (var y = -scaledOffset.Y; y < ourSize.Y; y += texSize.Y)
                        {
                            handle.DrawTextureRect(tex, UIBox2.FromDimensions(new Vector2(x, y), texSize));
                        }
                    }
                }
                else
                {
                    var origin = ((ourSize - texSize) / 2) + layer.Config.ControlHomePosition;
                    handle.DrawTextureRect(tex, UIBox2.FromDimensions(origin, texSize));
                }
            }
        }
        // Sunrise-end

        public enum LobbyGuiState : byte
        {
            /// <summary>
            ///  The default state, i.e., what's seen on launch.
            /// </summary>
            Default,
            /// <summary>
            ///  The character setup state.
            /// </summary>
            CharacterSetup
        }
    }
}
