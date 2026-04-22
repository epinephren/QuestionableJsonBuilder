namespace QuestionableJsonBuilder.ExportModels;

public sealed class ExportLastChecked
{
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public int Month { get; set; } = DateTime.UtcNow.Month;
    public int Day { get; set; } = DateTime.UtcNow.Day;
}
