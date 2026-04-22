namespace QuestionableJsonBuilder.ExportModels;

public sealed class ExportChatMessage
{
    public string? ExcelSheet { get; set; }
    public string Key { get; set; } = string.Empty;
}
