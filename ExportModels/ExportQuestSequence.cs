using System.Text.Json.Serialization;

namespace QuestionableJsonBuilder.ExportModels;

public sealed class ExportQuestSequence
{
    public byte Sequence { get; set; }
    public string? Comment { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ExportQuestStep>? Steps { get; set; }
}
