using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using QuestionableJsonBuilder.Debug;
using QuestionableJsonBuilder.Services;
using QuestionableJsonBuilder.Windows;

namespace QuestionableJsonBuilder;

public sealed class Plugin : IDalamudPlugin
{
    internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    internal static ICommandManager CommandManager { get; private set; } = null!;
    internal static IObjectTable ObjectTable { get; private set; } = null!;
    internal static IClientState ClientState { get; private set; } = null!;
    internal static IDataManager DataManager { get; private set; } = null!;
    internal static IPluginLog Log { get; private set; } = null!;
    internal static Configuration Configuration { get; private set; } = null!;
    private const string CommandName = "/qstb";
    private const int CurrentConfigVersion = 2;
    private const string WorkerUrl = "https://questionable-worker.epinephren.workers.dev/";

    private readonly WindowSystem windowSystem = new("QuestionableJsonBuilder");

    private readonly QuestWizardController controller;
    private readonly MainWindow mainWindow;
    private readonly ConfigWindow configWindow;
    private readonly QuestDebugWindow questDebugWindow;
    private readonly HelpWindow helpWindow;

    public Plugin(
        IDalamudPluginInterface pluginInterface,
        ICommandManager commandManager,
        IObjectTable objectTable,
        IClientState clientState,
        IDataManager dataManager,
        IPluginLog pluginLog)
    {
<<<<<<< main
        PluginInterface = pluginInterface;
        CommandManager = commandManager;
        ObjectTable = objectTable;
        ClientState = clientState;
        DataManager = dataManager;
        Log = pluginLog;
        Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(pluginInterface);
=======
        this.pluginInterface = pluginInterface;
        this.commandManager = commandManager;

        this.configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.configuration.Initialize(pluginInterface);
        
        if (this.configuration.Version < CurrentConfigVersion ||
            this.configuration.RemoteQuestIndexUrl.Contains("api.github.com", StringComparison.OrdinalIgnoreCase))
        {
            this.configuration.RemoteQuestIndexUrl = WorkerUrl;
            this.configuration.Version = CurrentConfigVersion;
            this.configuration.Save();
        }
        
>>>>>>> main

        this.controller = new QuestWizardController();
        this.helpWindow = new HelpWindow();
        this.mainWindow = new MainWindow(this.controller, this.OpenDebugUi, this.OpenHelpUi);
        this.configWindow = new ConfigWindow(this.controller);
        this.questDebugWindow = new QuestDebugWindow(this.controller);

        this.windowSystem.AddWindow(this.mainWindow);
        this.windowSystem.AddWindow(this.configWindow);
        this.windowSystem.AddWindow(this.questDebugWindow);
        this.windowSystem.AddWindow(this.helpWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(this.OnCommand)
        {
            HelpMessage = "Open the Questionable JSON Builder."
        });

        pluginInterface.UiBuilder.Draw += this.DrawUi;
        pluginInterface.UiBuilder.OpenMainUi += this.OpenMainUi;
        pluginInterface.UiBuilder.OpenConfigUi += this.OpenConfigUi;
    }

    private void OnCommand(string command, string arguments)
        => this.OpenMainUi();

    private void DrawUi() => this.windowSystem.Draw();
    private void OpenMainUi() => this.mainWindow.IsOpen = true;
    private void OpenConfigUi() => this.configWindow.IsOpen = true;
    private void OpenDebugUi() => this.questDebugWindow.IsOpen = true;
    private void OpenHelpUi() => this.helpWindow.IsOpen = true;

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= this.DrawUi;
        PluginInterface.UiBuilder.OpenMainUi -= this.OpenMainUi;
        PluginInterface.UiBuilder.OpenConfigUi -= this.OpenConfigUi;

        CommandManager.RemoveHandler(CommandName);
        this.windowSystem.RemoveAllWindows();
        this.controller.Dispose();
    }
}
