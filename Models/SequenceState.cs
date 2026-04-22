namespace QuestionableJsonBuilder.Models;

public sealed class SequenceState
{
    public byte Sequence { get; set; }
    public string? Comment { get; set; }
    public List<StepState> Steps { get; set; } = new();
}
