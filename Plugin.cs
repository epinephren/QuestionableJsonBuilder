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
    private const string CommandName = "/qstb";


    private readonly IDalamudPluginInterface pluginInterface;
    private readonly ICommandManager commandManager;
    private readonly WindowSystem windowSystem = new("QuestionableJsonBuilder");
    private readonly Configuration configuration;

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
        IDataManager dataManager)
    {
        this.pluginInterface = pluginInterface;
        this.commandManager = commandManager;

        this.configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.configuration.Initialize(pluginInterface);

        this.controller = new QuestWizardController(this.configuration, objectTable, clientState, dataManager);
        this.helpWindow = new HelpWindow();
        this.mainWindow = new MainWindow(this.controller, this.configuration, this.OpenDebugUi, this.OpenHelpUi);
        this.configWindow = new ConfigWindow(this.configuration, this.controller);
        this.questDebugWindow = new QuestDebugWindow(this.controller);

        this.windowSystem.AddWindow(this.mainWindow);
        this.windowSystem.AddWindow(this.configWindow);
        this.windowSystem.AddWindow(this.questDebugWindow);
        this.windowSystem.AddWindow(this.helpWindow);

        this.commandManager.AddHandler(CommandName, new CommandInfo(this.OnCommand)
        {
            HelpMessage = "Open the Questionable JSON Builder."
        });

        this.pluginInterface.UiBuilder.Draw += this.DrawUi;
        this.pluginInterface.UiBuilder.OpenMainUi += this.OpenMainUi;
        this.pluginInterface.UiBuilder.OpenConfigUi += this.OpenConfigUi;
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
        this.pluginInterface.UiBuilder.Draw -= this.DrawUi;
        this.pluginInterface.UiBuilder.OpenMainUi -= this.OpenMainUi;
        this.pluginInterface.UiBuilder.OpenConfigUi -= this.OpenConfigUi;

        this.commandManager.RemoveHandler(CommandName);
        this.windowSystem.RemoveAllWindows();
        this.controller.Dispose();
    }
}
