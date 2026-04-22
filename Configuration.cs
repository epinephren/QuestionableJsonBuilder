using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace QuestionableJsonBuilder;

[Serializable]
public sealed class Configuration : IPluginConfiguration
{
    [NonSerialized]
    private IDalamudPluginInterface? pluginInterface;

    public int Version { get; set; } = 1;
    public string DefaultAuthor { get; set; } = "YourName";
    public string DefaultExportDirectory { get; set; } = string.Empty;
    public bool PrettyPrintJson { get; set; } = true;
    public bool ShowAdvancedFields { get; set; }
    public string RemoteQuestIndexUrl { get; set; } = "https://api.github.com/repos/PunishXIV/Questionable/git/trees/new-main?recursive=1";

    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public void Save()
    {
        pluginInterface?.SavePluginConfig(this);
    }
}
