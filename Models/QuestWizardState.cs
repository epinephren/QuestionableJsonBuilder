namespace QuestionableJsonBuilder.Models;

public sealed class QuestWizardState
{
    public QuestIdentity? Quest { get; set; }
    public string Author { get; set; } = "YourName";
    public bool Interruptible { get; set; } = true;
    public string? QuestComment { get; set; }
    public bool QuestLocked { get; set; }
    public bool QuestCompletedConfirmed { get; set; }
    public List<SequenceState> MiddleSequences { get; set; } = new();
}
