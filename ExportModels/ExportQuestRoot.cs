using System.Text.Json.Serialization;

namespace QuestionableJsonBuilder.ExportModels;

public sealed class ExportQuestRoot
{
    [JsonPropertyName("$schema")]
    public string Schema { get; set; } = "https://qstxiv.github.io/schema/quest-v1.json";

    public List<string> Author { get; set; } = new();
    public bool Interruptible { get; set; }
    public string? Comment { get; set; }
    public ExportLastChecked LastChecked { get; set; } = new();
    public List<ExportQuestSequence> QuestSequence { get; set; } = new();
}
