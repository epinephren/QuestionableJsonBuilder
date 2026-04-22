using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Utility.Raii;
using QuestionableJsonBuilder.Services;

namespace QuestionableJsonBuilder.Windows;

public sealed class ConfigWindow : Window
{
    private readonly Configuration configuration;
    private readonly QuestWizardController controller;

    public ConfigWindow(Configuration configuration, QuestWizardController controller)
        : base("Questionable JSON Builder Settings")
    {
        this.configuration = configuration;
        this.controller = controller;
        this.Size = new Vector2(700, 360);
        this.SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void Draw()
    {
        ImGui.TextWrapped("Choose only the export directory. The filename remains locked to the Questionable-required format.");

        var defaultAuthor = this.configuration.DefaultAuthor;
        if (ImGui.InputText("Default Author", ref defaultAuthor, 128))
        {
            this.configuration.DefaultAuthor = defaultAuthor;
            if (string.IsNullOrWhiteSpace(this.controller.State.Author) || this.controller.State.Author == "YourName")
                this.controller.State.Author = defaultAuthor;
        }

        var defaultExportDirectory = this.configuration.DefaultExportDirectory ?? string.Empty;
        if (ImGui.InputText("Export Directory", ref defaultExportDirectory, 512))
            this.configuration.DefaultExportDirectory = string.IsNullOrWhiteSpace(defaultExportDirectory) ? string.Empty : defaultExportDirectory;

        if (ImGui.Button("Use Desktop Default"))
            this.configuration.DefaultExportDirectory = string.Empty;

        ImGui.SameLine();
        if (ImGui.Button("Save Settings"))
            this.configuration.Save();

        ImGui.Separator();

        var effectiveDirectory = string.IsNullOrWhiteSpace(this.configuration.DefaultExportDirectory)
            ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            : this.configuration.DefaultExportDirectory;

        ImGui.TextWrapped($"Active export directory: {effectiveDirectory}");

        using (ImRaii.PushColor(ImGuiCol.Text, new Vector4(1f, 0.25f, 0.25f, 1f)))
        {
          ImGui.TextWrapped(@"DO NOT CHANGE THE FILENAME. Only change the export directory. Example: C:\Users\Gamer01\Documents" + "\\");
        }

        ImGui.Separator();

        var remoteQuestIndexUrl = this.configuration.RemoteQuestIndexUrl;
        if (ImGui.InputText("Remote Questionable URL", ref remoteQuestIndexUrl, 512))
            this.configuration.RemoteQuestIndexUrl = remoteQuestIndexUrl;

        var prettyPrintJson = this.configuration.PrettyPrintJson;
        if (ImGui.Checkbox("Pretty Print JSON", ref prettyPrintJson))
            this.configuration.PrettyPrintJson = prettyPrintJson;

        var showAdvancedFields = this.configuration.ShowAdvancedFields;
        if (ImGui.Checkbox("Show Advanced Fields", ref showAdvancedFields))
            this.configuration.ShowAdvancedFields = showAdvancedFields;

        if (ImGui.Button("Refresh Lists"))
            this.controller.RefreshQuestIndex();

        ImGui.TextWrapped(this.controller.QuestSourceStatus);
    }
}
